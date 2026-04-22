using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class LiberarReservaOperadorCommandHandler : CommandHandlerBase<LiberarReservaOperadorCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;
        private readonly IRepository<Entity.Pago> _PagoRepository;
        private readonly IRepository<Entity.EstadoPago> _EstadoPagoRepository;
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public LiberarReservaOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LiberarReservaOperadorCommandValidator validator,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository,
            IRepository<Entity.Pago> PagoRepository,
            IRepository<Entity.EstadoPago> EstadoPagoRepository,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _PagoRepository = PagoRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(LiberarReservaOperadorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            var estadoPagoCancelado = await _EstadoPagoRepository.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_PAGO.Cancelado);

            var reserva = await _ReservaRepository.GetByAsync(r => r.IdReserva == request.LiberarDto.IdReserva && r.Activo,
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdCanchaNavigation.IdProveedorNavigation,
                r => r.IdCanchaNavigation.IdProveedorNavigation.ConfiguracionProveedor
            );

            if (reserva == null)
            {
                response.AddErrorResult("La reserva no existe o ha sido eliminada.");
                return response;
            }

            //Validar que el estado sea Pendiente o Confirmado
            if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente &&
                reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Confirmado)
            {
                response.AddErrorResult($"Solo las reservas pendientes o confirmadas pueden ser canceladas. Estado actual: {reserva.IdEstadoReservaNavigation.Nombre}");
                return response;
            }

            var pago = await _PagoRepository.GetByAsync(
                p => p.IdReserva == reserva.IdReserva && p.Activo,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                response.AddErrorResult("No se encontró el pago asociado a esta reserva.");
                return response;
            }

            if (reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado)
            {
                var configuracionProveedor = reserva.IdCanchaNavigation?.IdProveedorNavigation?.ConfiguracionProveedor;

                if (configuracionProveedor == null)
                {
                    response.AddErrorResult("No se encontró la configuración del proveedor.");
                    return response;
                }

                var fechaActual = DateTimeOffset.UtcNow;
                var horasHastaReserva = (reserva.FechaReserva - fechaActual).TotalHours; 

                decimal porcentajeDevolucion;
                if (horasHastaReserva > configuracionProveedor.TiempoLimiteCancelacion)
                    porcentajeDevolucion = configuracionProveedor.PorcentajeDevolucionCompleto;
                else
                    porcentajeDevolucion = configuracionProveedor.PorcentajeDevolucionParcial;

                var montoReembolsoEsperado = pago.Monto * (porcentajeDevolucion / 100);
                if (montoReembolsoEsperado > pago.MontoAdelanto)
                    montoReembolsoEsperado = pago.MontoAdelanto; // No reembolsar más de lo pagado

                //Validar que el monto coincida con el esperado (tolerancia de 0.01)
                var diferencia = Math.Abs(request.LiberarDto.MontoReembolso - montoReembolsoEsperado);
                if (diferencia > 0.01m)
                {
                    response.AddErrorResult($"El monto de reembolso no coincide con las políticas. Esperado: S/ {montoReembolsoEsperado:F2}, Recibido: S/ {request.LiberarDto.MontoReembolso:F2}");
                    return response;
                }

                pago.MontoReembolso = request.LiberarDto.MontoReembolso;
                pago.IdEstadoPago = estadoPagoCancelado.IdEstadoPago;

                await _PagoRepository.UpdateAsync(pago);
                await _PagoRepository.SaveAsync();
            }
            else
            {
                //Para reservas pendientes, validar que el monto de reembolso sea 0
                if (request.LiberarDto.MontoReembolso != 0)
                {
                    response.AddErrorResult("Las reservas pendientes no requieren reembolso. El monto debe ser 0.");
                    return response;
                }
            }

            var estadoReservaCancelado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Cancelado);

            if (estadoReservaCancelado == null)
            {
                response.AddErrorResult("Error del sistema: Estado de reserva no encontrado.");
                return response;
            }

            reserva.IdEstadoReserva = estadoReservaCancelado.IdEstadoReserva;
            reserva.FechaExpiracionPreReserva = null;

            await _ReservaRepository.UpdateAsync(reserva);
            await _ReservaRepository.SaveAsync();

            var reservaDto = _mapper?.Map<GetReservaDto>(reserva);
            response.UpdateData(reservaDto!);

            if (request.LiberarDto.MontoReembolso > 0)
            {
                response.AddOkResult($"Reserva {reserva.CodigoReserva} cancelada exitosamente. Reembolso procesado: S/ {request.LiberarDto.MontoReembolso:F2}. El horario está ahora disponible.");
            }
            else
            {
                response.AddOkResult($"Reserva {reserva.CodigoReserva} liberada exitosamente. El horario está ahora disponible.");
            }

            // TODO: Enviar notificación al cliente informando cancelación y reembolso

            return response;
        }
    }
}
