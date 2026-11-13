using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Reserva;

public class LiberarReservaOperadorCommandHandler : CommandHandlerBase<LiberarReservaOperadorCommand, GetReservaDto>
{
    private readonly IRepository<Entity.Reserva> _reservas;
    private readonly IRepository<Entity.EstadoReserva> _estadosReserva;
    private readonly IRepository<Entity.Pago> _pagos;
    private readonly IRepository<Entity.EstadoPago> _estadosPago;
    private readonly PagoReservaAccessService _access;
    private readonly MovimientoPagoService _movimientos;

    public LiberarReservaOperadorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator,
        LiberarReservaOperadorCommandValidator validator, IRepository<Entity.Reserva> reservas,
        IRepository<Entity.EstadoReserva> estadosReserva, IRepository<Entity.Pago> pagos,
        IRepository<Entity.EstadoPago> estadosPago, PagoReservaAccessService access, MovimientoPagoService movimientos)
        : base(unitOfWork, mapper, mediator, validator)
    {
        _reservas = reservas;
        _estadosReserva = estadosReserva;
        _pagos = pagos;
        _estadosPago = estadosPago;
        _access = access;
        _movimientos = movimientos;
    }

    public override async Task<ResponseDto<GetReservaDto>> HandleCommand(LiberarReservaOperadorCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetReservaDto>();
        var dto = request.LiberarDto;
        var reserva = await _reservas.GetByAsync(r => r.IdReserva == dto.IdReserva && r.Activo,
            r => r.IdEstadoReservaNavigation, r => r.IdCanchaNavigation.IdProveedorNavigation.ConfiguracionProveedor!);
        if (reserva == null || !await _access.PuedeAccederAsync(reserva) || !_access.PuedeOperarPorFecha(reserva))
        {
            response.AddErrorResult("Reserva no encontrada o sin permiso para cancelarla.");
            return response;
        }
        var pago = await _pagos.GetByAsync(p => p.IdReserva == reserva.IdReserva && p.Activo, p => p.IdEstadoPagoNavigation);
        var anterior = dto.IdOperacion == Guid.Empty ? null : await _movimientos.BuscarOperacionAsync(dto.IdOperacion);
        if (anterior != null)
        {
            var metodo = await _movimientos.ObtenerMetodoAsync(dto.CodigoMetodoPago);
            if (pago == null || anterior.IdPago != pago.IdPago || anterior.Tipo != "REEMBOLSO" ||
                anterior.Importe != dto.MontoReembolso || anterior.IdUsuarioResponsable != _access.IdUsuario ||
                anterior.IdMetodoPago != metodo?.IdMetodoPago ||
                anterior.ReferenciaOperacion != Normalizar(dto.ReferenciaOperacion) || anterior.Motivo != Normalizar(dto.MotivoLiberacion))
            {
                response.AddErrorResult("La operación ya se utilizó con otros datos.");
                return response;
            }
            response.UpdateData(_mapper!.Map<GetReservaDto>(reserva));
            response.AddOkResult("El reembolso ya estaba registrado; no se duplicó.");
            return response;
        }
        if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente &&
            reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Confirmado)
        {
            response.AddErrorResult("Solo se pueden cancelar reservas pendientes o confirmadas.");
            return response;
        }
        if ((dto.RevisionPago ?? 0) != (pago?.Revision ?? 0))
        {
            response.AddErrorResult("El pago cambió. Recarga el detalle antes de cancelar o devolver dinero.");
            return response;
        }
        decimal esperado = 0;
        if (pago?.MontoAdelanto > 0)
        {
            var config = reserva.IdCanchaNavigation.IdProveedorNavigation.ConfiguracionProveedor;
            if (config == null)
            {
                response.AddErrorResult("No se encontró la configuración del proveedor.");
                return response;
            }
            var horas = (reserva.FechaReserva - DateTimeOffset.UtcNow).TotalHours;
            var porcentaje = horas > config.TiempoLimiteCancelacion ? config.PorcentajeDevolucionCompleto : config.PorcentajeDevolucionParcial;
            esperado = decimal.Round(Math.Min(pago.Monto * porcentaje / 100, pago.MontoAdelanto - (pago.MontoReembolso ?? 0)), 2,
                MidpointRounding.AwayFromZero);
        }
        if (dto.MontoReembolso != esperado)
        {
            response.AddErrorResult($"El reembolso debe ser S/ {esperado:F2}, de acuerdo con la política vigente.");
            return response;
        }
        if (dto.MontoReembolso > 0)
        {
            var metodo = await _movimientos.ObtenerMetodoAsync(dto.CodigoMetodoPago);
            if (metodo == null)
            {
                response.AddErrorResult("Selecciona el método de devolución.");
                return response;
            }
            await _movimientos.RegistrarReembolsoAsync(pago!, dto.MontoReembolso, metodo.IdMetodoPago,
                dto.IdOperacion, dto.MotivoLiberacion, dto.ReferenciaOperacion);
        }
        var cancelado = await _estadosReserva.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_RESERVA.Cancelado)
            ?? throw new InvalidOperationException("Estado de reserva cancelada no configurado.");
        if (pago != null)
        {
            var estadoPago = await _estadosPago.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_PAGO.Cancelado)
                ?? throw new InvalidOperationException("Estado de pago cancelado no configurado.");
            pago.IdEstadoPago = estadoPago.IdEstadoPago;
            pago.IdEstadoPagoNavigation = estadoPago;
            pago.MontoPendiente = 0;
            pago.Revision++;
            await _pagos.UpdateAsync(pago);
        }
        reserva.IdEstadoReserva = cancelado.IdEstadoReserva;
        reserva.IdEstadoReservaNavigation = cancelado;
        reserva.FechaExpiracionPreReserva = null;
        await _reservas.UpdateAsync(reserva);
        response.UpdateData(_mapper!.Map<GetReservaDto>(reserva));
        response.AddOkResult(dto.MontoReembolso > 0 ? "Reserva cancelada y devolución registrada con historial." : "Reserva cancelada.");
        return response;
    }

    private static string? Normalizar(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
