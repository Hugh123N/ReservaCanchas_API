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
    private readonly IRepository<Entity.Reserva> _reservas;

    public MovimientoPagoService(IRepository<Entity.Pago> pagos, IRepository<Entity.MovimientoPago> movimientos,
        IRepository<Entity.MetodoPago> metodos, IRepository<Entity.EstadoPago> estadosPago,
        IRepository<Entity.EstadoReserva> estadosReserva, PagoReservaAccessService access,
        IRepository<Entity.ConfiguracionProveedor> configuraciones, IRepository<Entity.Reserva> reservas)
    {
        _pagos = pagos;
        _movimientos = movimientos;
        _metodos = metodos;
        _estadosPago = estadosPago;
        _estadosReserva = estadosReserva;
        _access = access;
        _configuraciones = configuraciones;
        _reservas = reservas;
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
        response.UpdateData(await ResumenAsync(pago.IdReservaNavigation, pago));
        response.AddOkResult("Historial obtenido.");
        return response;
    }

    public async Task<ResponseDto<GetHistorialPagoDto>> ObtenerPorReservaAsync(int idReserva)
    {
        var response = new ResponseDto<GetHistorialPagoDto>();
        var reserva = await CargarReservaAsync(idReserva);
        if (reserva == null || !await _access.PuedeAccederAsync(reserva))
        {
            response.AddErrorResult("Reserva no encontrada o sin acceso.");
            return response;
        }
        response.UpdateData(await ResumenAsync(reserva, reserva.Pago.FirstOrDefault(p => p.Activo)));
        response.AddOkResult("Historial obtenido.");
        return response;
    }

    public async Task<ResponseDto<GetHistorialPagoDto>> RegistrarAsync(RegistrarMovimientoPagoDto dto, bool correccion)
    {
        var response = new ResponseDto<GetHistorialPagoDto>();
        var pago = dto.IdPago > 0 ? await CargarAsync(dto.IdPago) : null;
        var reserva = pago?.IdReservaNavigation ?? (dto.IdReserva.HasValue ? await CargarReservaAsync(dto.IdReserva.Value) : null);
        if (reserva == null || !await _access.PuedeAccederAsync(reserva) ||
            (dto.IdReserva.HasValue && dto.IdReserva != reserva.IdReserva))
        {
            response.AddErrorResult("Reserva no encontrada o sin acceso.");
            return response;
        }
        pago ??= reserva.Pago.FirstOrDefault(p => p.Activo);
        if (correccion && (!_access.EsProveedor || pago == null))
        {
            response.AddErrorResult("Solo el proveedor puede corregir un pago existente.");
            return response;
        }
        var tipo = correccion ? "CORRECCION" : "COBRO";
        var anterior = await _movimientos.GetByAsync(m => m.IdOperacion == dto.IdOperacion);
        if (anterior != null)
        {
            var metodoAnterior = anterior.IdMetodoPago.HasValue
                ? await _metodos.GetByAsNoTrackingAsync(m => m.IdMetodoPago == anterior.IdMetodoPago.Value) : null;
            if (pago == null || anterior.IdPago != pago.IdPago || anterior.Tipo != tipo ||
                anterior.IdUsuarioResponsable != _access.IdUsuario ||
                (correccion ? anterior.MontoResultante : anterior.Importe) != dto.Monto ||
                anterior.Motivo != Normalizar(dto.Motivo) || anterior.ReferenciaOperacion != Normalizar(dto.ReferenciaOperacion) ||
                (!correccion && metodoAnterior?.Codigo != dto.CodigoMetodoPago))
            {
                response.AddErrorResult("La operación ya se utilizó con otros datos.");
                return response;
            }
            response.UpdateData(await ResumenAsync(reserva, pago));
            response.AddOkResult("La operación ya estaba registrada; no se duplicó.");
            return response;
        }
        if (!EstaVigente(reserva, pago) || !_access.PuedeOperarPorFecha(reserva))
        {
            response.AddErrorResult("No se puede operar sobre esta reserva cancelada, expirada o fuera de fecha.");
            return response;
        }
        if ((pago?.Revision ?? 0) != dto.Revision || (dto.IdPago == 0 && pago != null))
        {
            response.AddErrorResult("El pago cambió mientras lo revisabas. Actualiza el detalle antes de continuar.");
            return response;
        }
        var total = pago?.Monto ?? reserva.MontoTotal;
        var cobrado = pago?.MontoAdelanto ?? 0;
        var resultado = correccion ? dto.Monto : cobrado + dto.Monto;
        if (dto.Monto < 0 || (!correccion && dto.Monto == 0) || decimal.Round(dto.Monto, 2) != dto.Monto ||
            resultado < (pago?.MontoReembolso ?? 0) || resultado > total || resultado == cobrado)
        {
            response.AddErrorResult("El importe debe respetar el saldo, los reembolsos y un máximo de dos decimales.");
            return response;
        }
        var metodo = correccion ? null : await ObtenerMetodoAsync(dto.CodigoMetodoPago);
        if (!correccion && metodo == null)
        {
            response.AddErrorResult("Selecciona un método de cobro presencial válido.");
            return response;
        }
        var nuevoPago = pago == null;
        if (pago == null)
        {
            pago = new Entity.Pago { IdReserva = reserva.IdReserva,
                Monto = total, Moneda = Constants.CURRENCY.PEN, Activo = true };
        }
        else
        {
            await IncorporarSaldoHistoricoAsync(pago);
        }
        var movimiento = CrearMovimiento(tipo, resultado - cobrado, cobrado, resultado,
            metodo?.IdMetodoPago, dto.IdOperacion, dto.Motivo, dto.ReferenciaOperacion);
        pago.MontoAdelanto = resultado;
        pago.MontoPendiente = pago.Monto - resultado;
        pago.Revision++;
        await ActualizarEstadoAsync(pago);
        var config = await _configuraciones.GetByAsNoTrackingAsync(c => c.IdProveedor == reserva.IdCanchaNavigation.IdProveedor && c.Activo);
        var minimo = total * (config?.PorcentajeAdelantoMinimo ?? 0) / 100;
        if (resultado > 0 && resultado >= minimo && reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente)
        {
            var confirmado = await _estadosReserva.GetByAsync(e => e.Codigo == Constants.ESTADO_RESERVA.Confirmado && e.Activo)
                ?? throw new InvalidOperationException("Estado de reserva no configurado.");
            reserva.IdEstadoReserva = confirmado.IdEstadoReserva;
            reserva.IdEstadoReservaNavigation = confirmado;
            reserva.FechaConfirmacion ??= DateTimeOffset.UtcNow;
            reserva.FechaExpiracionPreReserva = null;
        }
        await _reservas.UpdateAsync(reserva);
        // AddAsync guarda inmediatamente: la cabecera debe tener estado y montos válidos antes de insertarla.
        if (nuevoPago)
        {
            pago.MovimientoPago.Add(movimiento);
            await _pagos.AddAsync(pago);
            // Vincular después del alta evita que la auditoría de creación recorra la reserva existente.
            pago.IdReservaNavigation = reserva;
            if (!reserva.Pago.Contains(pago)) reserva.Pago.Add(pago);
        }
        else
        {
            await _pagos.UpdateAsync(pago);
            await AgregarAsync(pago, movimiento);
        }
        response.UpdateData(new GetHistorialPagoDto { IdPago = pago.IdPago, IdReserva = reserva.IdReserva,
            Revision = pago.Revision, Monto = total, MontoPagado = resultado, Saldo = pago.MontoPendiente,
            MontoReembolsado = pago.MontoReembolso ?? 0, MontoNeto = resultado - (pago.MontoReembolso ?? 0) });
        response.AddOkResult(correccion ? "Corrección registrada con historial." : "Cobro registrado.");
        return response;
    }

    public Entity.MovimientoPago CrearMovimiento(string tipo, decimal importe, decimal anterior, decimal resultante,
        int? idMetodo, Guid idOperacion, string? motivo, string? referencia)
    {
        if (!_access.IdUsuario.HasValue || _access.IdUsuario == Guid.Empty)
            throw new InvalidOperationException("No se pudo identificar al responsable del movimiento.");
        return new Entity.MovimientoPago { IdOperacion = idOperacion, Tipo = tipo, Importe = importe,
            MontoAnterior = anterior, MontoResultante = resultante, IdMetodoPago = idMetodo,
            IdUsuarioResponsable = _access.IdUsuario, RolResponsable = _access.Rol,
            Motivo = Normalizar(motivo), ReferenciaOperacion = Normalizar(referencia) };
    }

    public async Task<Entity.MetodoPago?> ObtenerMetodoAsync(string? codigo)
    {
        if (codigo != Constants.METODO_PAGO.Efectivo && codigo != Constants.METODO_PAGO.Transferencia &&
            codigo != Constants.METODO_PAGO.Yape && codigo != Constants.METODO_PAGO.Plin) return null;
        return await _metodos.GetByAsync(m => m.Codigo == codigo && m.Activo);
    }

    public async Task RegistrarReembolsoAsync(Entity.Pago pago, decimal importe, int idMetodo, Guid idOperacion,
        string? motivo, string? referencia)
    {
        var anterior = pago.MontoReembolso ?? 0;
        if (importe <= 0 || anterior + importe > pago.MontoAdelanto)
            throw new InvalidOperationException("El reembolso supera el dinero disponible para devolver.");
        await IncorporarSaldoHistoricoAsync(pago);
        // Los acumulados anterior/resultante de un reembolso corresponden al total devuelto, no al cobrado.
        await AgregarAsync(pago, CrearMovimiento("REEMBOLSO", importe, anterior, anterior + importe,
            idMetodo, idOperacion, motivo, referencia));
        pago.MontoReembolso = anterior + importe;
    }

    public Task<Entity.MovimientoPago?> BuscarOperacionAsync(Guid idOperacion) =>
        _movimientos.GetByAsync(m => m.IdOperacion == idOperacion);

    private async Task IncorporarSaldoHistoricoAsync(Entity.Pago pago)
    {
        if (pago.MontoAdelanto <= 0 || pago.IdPago == 0 || (await _movimientos.FindByAsync(m => m.IdPago == pago.IdPago)).Any()) return;
        await AgregarAsync(pago, new Entity.MovimientoPago { IdOperacion = Guid.NewGuid(), Tipo = "SALDO_INICIAL",
            Importe = pago.MontoAdelanto, MontoAnterior = 0, MontoResultante = pago.MontoAdelanto,
            Motivo = "Acumulado histórico sin responsable ni fecha de cobro verificables." });
    }

    private async Task AgregarAsync(Entity.Pago pago, Entity.MovimientoPago movimiento)
    {
        movimiento.IdPago = pago.IdPago;
        if (pago.IdPago == 0) movimiento.IdPagoNavigation = pago;
        await _movimientos.AddAsync(movimiento);
    }

    private async Task ActualizarEstadoAsync(Entity.Pago pago)
    {
        var codigo = pago.MontoAdelanto == 0 ? Constants.ESTADO_PAGO.Pendiente :
            pago.MontoAdelanto == pago.Monto ? Constants.ESTADO_PAGO.Pagado : Constants.ESTADO_PAGO.Parcial;
        var estado = await _estadosPago.GetByAsync(e => e.Codigo == codigo && e.Activo)
            ?? throw new InvalidOperationException("Estado de pago no configurado.");
        pago.IdEstadoPago = estado.IdEstadoPago;
        pago.IdEstadoPagoNavigation = estado;
    }

    private Task<Entity.Pago?> CargarAsync(int idPago) => _pagos.GetByAsync(p => p.IdPago == idPago && p.Activo,
        p => p.IdReservaNavigation!.IdCanchaNavigation, p => p.IdReservaNavigation!.IdEstadoReservaNavigation,
        p => p.IdEstadoPagoNavigation);

    private async Task<Entity.Reserva?> CargarReservaAsync(int idReserva)
    {
        var reserva = await _reservas.GetByAsync(r => r.IdReserva == idReserva && r.Activo,
            r => r.IdCanchaNavigation, r => r.IdEstadoReservaNavigation, r => r.Pago);
        var pago = reserva?.Pago.FirstOrDefault(p => p.Activo);
        if (pago != null && pago.IdEstadoPagoNavigation == null)
            pago.IdEstadoPagoNavigation = (await _estadosPago.GetByAsync(e => e.IdEstadoPago == pago.IdEstadoPago))!;
        return reserva;
    }

    private static bool EstaVigente(Entity.Reserva reserva, Entity.Pago? pago) => reserva.Activo &&
        (reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente ||
         reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado) &&
        (pago == null || pago.IdEstadoPagoNavigation?.Codigo != Constants.ESTADO_PAGO.Cancelado) &&
        (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente ||
         !reserva.FechaExpiracionPreReserva.HasValue || reserva.FechaExpiracionPreReserva > DateTimeOffset.UtcNow);

    private async Task<GetHistorialPagoDto> ResumenAsync(Entity.Reserva reserva, Entity.Pago? pago)
    {
        var movimientos = pago == null ? new List<Entity.MovimientoPago>() :
            (await _movimientos.FindByAsNoTrackingAsync(m => m.IdPago == pago.IdPago && m.Activo)).OrderBy(m => m.IdMovimientoPago).ToList();
        var metodos = (await _metodos.FindByAsNoTrackingAsync(m => true)).ToDictionary(m => m.IdMetodoPago, m => m.Nombre);
        var vigente = EstaVigente(reserva, pago) && _access.PuedeOperarPorFecha(reserva);
        var total = pago?.Monto ?? reserva.MontoTotal;
        var cobrado = pago?.MontoAdelanto ?? 0;
        var devuelto = pago?.MontoReembolso ?? 0;
        return new GetHistorialPagoDto { IdPago = pago?.IdPago ?? 0, IdReserva = reserva.IdReserva,
            Revision = pago?.Revision ?? 0, Monto = total, MontoPagado = cobrado,
            Saldo = pago?.MontoPendiente ?? total, MontoReembolsado = devuelto, MontoNeto = cobrado - devuelto,
            PuedeCobrar = vigente && cobrado < total, PuedeCorregir = vigente && pago != null && _access.EsProveedor,
            Movimientos = movimientos.Select(m => new GetMovimientoPagoDto { IdMovimientoPago = m.IdMovimientoPago,
                Tipo = m.Tipo, Importe = m.Importe, MontoAnterior = m.MontoAnterior, MontoResultante = m.MontoResultante,
                IdMetodoPago = m.IdMetodoPago, MetodoPago = m.IdMetodoPago.HasValue ? metodos.GetValueOrDefault(m.IdMetodoPago.Value) : null,
                IdUsuarioResponsable = m.IdUsuarioResponsable, RolResponsable = m.RolResponsable,
                ReferenciaOperacion = m.ReferenciaOperacion, Motivo = m.Motivo, Usuario = m.UserNameCreate, Fecha = m.CreateDate }).ToList() };
    }

    private static string? Normalizar(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
