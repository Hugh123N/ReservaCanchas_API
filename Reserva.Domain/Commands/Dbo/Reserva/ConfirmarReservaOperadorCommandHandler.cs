using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Notificacion;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class ConfirmarReservaOperadorCommandHandler : CommandHandlerBase<ConfirmarReservaOperadorCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.Pago> _PagoRepository;
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _EstadoPagoRepository;
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;
        private readonly INotificacionService _notificacionService;

        public ConfirmarReservaOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ConfirmarReservaOperadorCommandValidator validator,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.Pago> PagoRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository,
            IRepository<Entity.EstadoPago> EstadoPagoRepository,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository,
            INotificacionService notificacionService
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
            _PagoRepository = PagoRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _CanchaRepository = CanchaRepository;
            _DetalleReservaRepository = DetalleReservaRepository;
            _notificacionService = notificacionService;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(
            ConfirmarReservaOperadorCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            var reserva = await _ReservaRepository.GetByAsync(
                r => r.IdReserva == request.ConfirmarDto.IdReserva && r.Activo,
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.Pago
            );

            if (reserva == null)
            {
                response.AddErrorResult("La reserva no existe o ha sido eliminada.");
                return response;
            }

            if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente)
            {
                response.AddErrorResult($"La reserva no puede ser confirmada. Estado actual: {reserva.IdEstadoReservaNavigation.Nombre}");
                return response;
            }

            //Validar que la reserva no haya expirado
            if (reserva.FechaExpiracionPreReserva.HasValue && reserva.FechaExpiracionPreReserva.Value < DateTimeOffset.UtcNow)
            {
                response.AddErrorResult("La reserva ha expirado y no puede ser confirmada.");
                return response;
            }

            var pago = reserva.Pago.FirstOrDefault(p => p.Activo);
            if (pago == null)
            {
                response.AddErrorResult("No se encontró el pago asociado a la reserva.");
                return response;
            }

            var cancha = reserva.IdCanchaNavigation;
            decimal porcentajeMinimoAdelanto = cancha?.IdProveedorNavigation.ConfiguracionProveedor.PorcentajeAdelantoMinimo ?? 0;

            //Procesar adelanto si fue proporcionado
            decimal montoAdelanto = request.ConfirmarDto.MontoAdelanto ?? 0;
            decimal montoTotal = pago.Monto;

            if (montoAdelanto > montoTotal)
            {
                response.AddErrorResult($"El adelanto (S/ {montoAdelanto:F2}) no puede ser mayor al monto total (S/ {montoTotal:F2}).");
                return response;
            }

            // Validar porcentaje mínimo de adelanto si se proporciona adelanto
            if (montoAdelanto > 0)
            {
                decimal porcentajeAdelanto = (montoAdelanto / montoTotal) * 100;
                if (porcentajeAdelanto < porcentajeMinimoAdelanto)
                {
                    response.AddErrorResult(
                        $"El adelanto debe ser al menos {porcentajeMinimoAdelanto}% del total. " +
                        $"Mínimo requerido: S/ {(montoTotal * porcentajeMinimoAdelanto / 100):F2}");
                    return response;
                }
            }

            // 7. Actualizar el pago
            pago.MontoAdelanto = montoAdelanto;
            pago.MontoPendiente = montoTotal - montoAdelanto;
            pago.NumeroReferencia = request.ConfirmarDto.NumeroRecibo;

            Entity.EstadoPago? estadoPago;

            if (montoAdelanto >= montoTotal)
            {
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pagado);
            }
            else if (montoAdelanto > 0)
            {
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Parcial);
            }
            else
            {
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pendiente);
            }

            if (estadoPago == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago no encontrado.");
                return response;
            }

            pago.IdEstadoPago = estadoPago.IdEstadoPago;

            await _PagoRepository.UpdateAsync(pago);
            await _PagoRepository.SaveAsync();

            var estadoConfirmado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Confirmado);

            if (estadoConfirmado == null)
            {
                response.AddErrorResult("Error del sistema: Estado de reserva no encontrado.");
                return response;
            }

            reserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;
            reserva.FechaExpiracionPreReserva = null;

            await _ReservaRepository.UpdateAsync(reserva);
            await _ReservaRepository.SaveAsync();

            var reservaDto = _mapper?.Map<GetReservaDto>(reserva);

            response.UpdateData(reservaDto!);
            response.AddOkResult(
                $"Reserva confirmada exitosamente. " +
                $"Código: {reserva.CodigoReserva}. " +
                $"Pago: {estadoPago.Nombre} " +
                $"(Adelanto: S/ {montoAdelanto:F2}, Pendiente: S/ {pago.MontoPendiente:F2})");

            // Enviar notificación al cliente informando confirmación
            try
            {
                var cliente = await _ReservaRepository.FindAll()
                    .Where(r => r.IdReserva == reserva.IdReserva)
                    .Select(r => r.IdClienteNavigation)
                    .FirstOrDefaultAsync();

                if (cliente != null && cancha != null)
                {
                    var detallesConHorarios = await _DetalleReservaRepository.FindByAsNoTrackingAsync(
                        d => d.IdReserva == reserva.IdReserva && d.Activo,
                        d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                        d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
                    );

                    var horariosLista = detallesConHorarios
                        .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                                 && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                        .Select(d => {
                            var horaInicio = d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
                            var horaFin = d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                            return (inicio: horaInicio, fin: horaFin);
                        })
                        .OrderBy(h => h.inicio)
                        .ToList();

                    var horariosFormateado = NotificacionService.FormatearHorariosConsecutivos(horariosLista);

                    await _notificacionService.NotificarReservaConfirmadaAsync(
                        reserva,
                        cancha,
                        cliente,
                        pago,
                        horariosFormateado
                    );
                }
            }
            catch (Exception)
            {
                // No fallar la confirmación si falla la notificación
            }

            return response;
        }
    }
}
