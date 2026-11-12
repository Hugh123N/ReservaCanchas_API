using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.Pago;

public class MovimientoPagoService
{
    private readonly IRepository<Entity.Pago> _pagos;
    private readonly IRepository<Entity.MovimientoPago> _movimientos;
    private readonly IRepository<Entity.MetodoPago> _metodos;
    private readonly IRepository<Entity.EstadoPago> _estadosPago;
    private readonly IRepository<Entity.EstadoReserva> _estadosReserva;
    private readonly PagoReservaAccessService _access;
    private readonly IRepository<Entity.ConfiguracionProveedor> _configuraciones;

    public MovimientoPagoService(IRepository<Entity.Pago> pagos, IRepository<Entity.MovimientoPago> movimientos,
        IRepository<Entity.MetodoPago> metodos, IRepository<Entity.EstadoPago> estadosPago,
        IRepository<Entity.EstadoReserva> estadosReserva, PagoReservaAccessService access,
        IRepository<Entity.ConfiguracionProveedor> configuraciones)
    {
        _pagos = pagos;
        _movimientos = movimientos;
        _metodos = metodos;
        _estadosPago = estadosPago;
        _estadosReserva = estadosReserva;
        _access = access;
        _configuraciones = configuraciones;
    }

    public async Task<ResponseDto<GetHistorialPagoDto>> ObtenerAsync(int idPago)
    {
        var response = new ResponseDto<GetHistorialPagoDto>();
        var pago = await CargarAsync(idPago);
        if (pago?.IdReservaNavigation == null || !await _access.PuedeAccederAsync(pago.IdReservaNavigation))
        {
            response.AddErrorResult("Pago no encontrado o sin acceso.");
            return response;
        }
        response.UpdateData(await ResumenAsync(pago));
        response.AddOkResult("Historial obtenido.");
        return response;
    }

    public async Task<ResponseDto<GetHistorialPagoDto>> RegistrarAsync(RegistrarMovimientoPagoDto dto, bool correccion)
    {
        var response = new ResponseDto<GetHistorialPagoDto>();
        var pago = await CargarAsync(dto.IdPago);
        var reserva = pago?.IdReservaNavigation;
        if (reserva == null || !await _access.PuedeAccederAsync(reserva))
        {
            response.AddErrorResult("Pago no encontrado o sin acceso.");
            return response;
        }
        if (correccion && !_access.EsProveedor)
        {
            response.AddErrorResult("Solo el proveedor puede corregir registros de pago.");
            return response;
        }
        var tipo = correccion ? "CORRECCION" : "COBRO";
        var anterior = await _movimientos.GetByAsync(m => m.IdOperacion == dto.IdOperacion);
        if (anterior != null)
        {
            var metodoAnterior = anterior.IdMetodoPago.HasValue
                ? await _metodos.GetByAsNoTrackingAsync(m => m.IdMetodoPago == anterior.IdMetodoPago.Value) : null;
            if (anterior.IdPago != dto.IdPago || anterior.Tipo != tipo ||
                (correccion ? anterior.MontoResultante : anterior.Importe) != dto.Monto ||
                anterior.Motivo != dto.Motivo?.Trim() || (!correccion && metodoAnterior?.Codigo != dto.CodigoMetodoPago))
            {
                response.AddErrorResult("La operación ya se utilizó con otros datos.");
                return response;
            }
            response.UpdateData(await ResumenAsync(pago!));
            response.AddOkResult("La operación ya estaba registrada; no se duplicó.");
            return response;
        }
        if (!EstaVigente(pago!) || !_access.PuedeOperarPorFecha(reserva))
        {
            response.AddErrorResult("No se puede operar sobre esta reserva cancelada, expirada o fuera de fecha.");
            return response;
        }
        if (pago!.Revision != dto.Revision)
        {
            response.AddErrorResult("El pago cambió mientras lo revisabas. Actualiza el detalle antes de continuar.");
            return response;
        }
        var resultado = correccion ? dto.Monto : pago.MontoAdelanto + dto.Monto;
        if (resultado < 0 || resultado > pago.Monto || resultado == pago.MontoAdelanto)
        {
            response.AddErrorResult("El importe resultante debe estar entre cero y el total, y ser distinto del registrado.");
            return response;
        }
        Entity.MetodoPago? metodo = null;
        if (!correccion)
        {
            var codigo = dto.CodigoMetodoPago;
            if (codigo != Constants.METODO_PAGO.Efectivo && codigo != Constants.METODO_PAGO.Transferencia &&
                codigo != Constants.METODO_PAGO.Yape && codigo != Constants.METODO_PAGO.Plin)
            {
                response.AddErrorResult("Selecciona un método de cobro presencial válido.");
                return response;
            }
            metodo = await _metodos.GetByAsync(m => m.Codigo == codigo && m.Activo);
            if (metodo == null)
            {
                response.AddErrorResult("El método de pago no está disponible.");
                return response;
            }
        }
        if (!(await _movimientos.FindByAsync(m => m.IdPago == pago.IdPago)).Any())
        {
            await _movimientos.AddAsync(new Entity.MovimientoPago
            {
                IdPago = pago.IdPago, IdOperacion = Guid.NewGuid(), Tipo = "SALDO_INICIAL",
                Importe = pago.MontoAdelanto, MontoAnterior = 0, MontoResultante = pago.MontoAdelanto,
                IdMetodoPago = pago.IdMetodoPago, Motivo = "Acumulado previo al historial de movimientos."
            });
        }
        await _movimientos.AddAsync(new Entity.MovimientoPago
        {
            IdPago = pago.IdPago, IdOperacion = dto.IdOperacion, Tipo = tipo,
            Importe = resultado - pago.MontoAdelanto, MontoAnterior = pago.MontoAdelanto,
            MontoResultante = resultado, IdMetodoPago = metodo?.IdMetodoPago, Motivo = dto.Motivo?.Trim()
        });
        pago.MontoAdelanto = resultado;
        pago.MontoPendiente = pago.Monto - resultado;
        pago.Revision++;
        var codigoEstado = resultado == 0 ? Constants.ESTADO_PAGO.Pendiente :
            resultado == pago.Monto ? Constants.ESTADO_PAGO.Pagado : Constants.ESTADO_PAGO.Parcial;
        var estado = await _estadosPago.GetByAsync(e => e.Codigo == codigoEstado && e.Activo);
        if (estado == null) throw new InvalidOperationException("Estado de pago no configurado.");
        pago.IdEstadoPago = estado.IdEstadoPago;
        // Una corrección refleja dinero real; no se exige un adelanto mínimo ficticio.
        var config = await _configuraciones.GetByAsNoTrackingAsync(c => c.IdProveedor == reserva.IdCanchaNavigation.IdProveedor && c.Activo);
        var minimo = pago.Monto * (config?.PorcentajeAdelantoMinimo ?? 0) / 100;
        if (resultado > 0 && resultado >= minimo && reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente)
        {
            var confirmado = await _estadosReserva.GetByAsync(e => e.Codigo == Constants.ESTADO_RESERVA.Confirmado && e.Activo);
            if (confirmado == null) throw new InvalidOperationException("Estado de reserva no configurado.");
            reserva.IdEstadoReserva = confirmado.IdEstadoReserva;
            reserva.FechaConfirmacion ??= DateTimeOffset.UtcNow;
            reserva.FechaExpiracionPreReserva = null;
        }
        await _pagos.UpdateAsync(pago);
        // El cliente recarga el historial después del commit de UnitOfWork.
        response.UpdateData(new GetHistorialPagoDto { IdPago = pago.IdPago, Revision = pago.Revision,
            Monto = pago.Monto, MontoPagado = pago.MontoAdelanto, Saldo = pago.MontoPendiente });
        response.AddOkResult(correccion ? "Corrección registrada con historial." : "Cobro registrado.");
        return response;
    }

    private Task<Entity.Pago?> CargarAsync(int idPago) => _pagos.GetByAsync(p => p.IdPago == idPago && p.Activo,
        p => p.IdReservaNavigation!.IdCanchaNavigation, p => p.IdReservaNavigation!.IdEstadoReservaNavigation,
        p => p.IdEstadoPagoNavigation);

    private static bool EstaVigente(Entity.Pago pago) =>
        pago.IdEstadoPagoNavigation.Codigo != Constants.ESTADO_PAGO.Cancelado &&
        (pago.IdReservaNavigation!.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente ||
         pago.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado) &&
        (pago.IdReservaNavigation.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente ||
         !pago.IdReservaNavigation.FechaExpiracionPreReserva.HasValue ||
         pago.IdReservaNavigation.FechaExpiracionPreReserva > DateTimeOffset.UtcNow);

    private async Task<GetHistorialPagoDto> ResumenAsync(Entity.Pago pago)
    {
        var movimientos = (await _movimientos.FindByAsNoTrackingAsync(m => m.IdPago == pago.IdPago && m.Activo))
            .OrderBy(m => m.IdMovimientoPago).ToList();
        var metodos = (await _metodos.FindByAsNoTrackingAsync(m => true)).ToDictionary(m => m.IdMetodoPago, m => m.Nombre);
        var vigente = EstaVigente(pago) && _access.PuedeOperarPorFecha(pago.IdReservaNavigation!);
        return new GetHistorialPagoDto
        {
            IdPago = pago.IdPago, Revision = pago.Revision, Monto = pago.Monto,
            MontoPagado = pago.MontoAdelanto, Saldo = pago.Monto - pago.MontoAdelanto,
            PuedeCobrar = vigente && pago.MontoAdelanto < pago.Monto,
            PuedeCorregir = vigente && _access.EsProveedor,
            Movimientos = movimientos.Select(m => new GetMovimientoPagoDto
            {
                IdMovimientoPago = m.IdMovimientoPago, Tipo = m.Tipo, Importe = m.Importe,
                MontoAnterior = m.MontoAnterior, MontoResultante = m.MontoResultante,
                MetodoPago = m.IdMetodoPago.HasValue ? metodos.GetValueOrDefault(m.IdMetodoPago.Value) : null,
                Motivo = m.Motivo, Usuario = m.UserNameCreate, Fecha = m.CreateDate
            }).ToList()
        };
    }
}
