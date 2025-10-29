using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CompletarPagoCommandHandler : CommandHandlerBase<CompletarPagoCommand, GetPagoDto>
    {
        private readonly IRepository<Entity.Pago> _PagoRepository;
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _EstadoPagoRepository;
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;
        private readonly IRepository<Entity.MetodoPago> _MetodoPagoRepository;

        public CompletarPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CompletarPagoCommandValidator validator,
            IRepository<Entity.Pago> PagoRepository,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.EstadoPago> EstadoPagoRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository,
            IRepository<Entity.MetodoPago> MetodoPagoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PagoRepository = PagoRepository;
            _ReservaRepository = ReservaRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        public override async Task<ResponseDto<GetPagoDto>> HandleCommand(
            CompletarPagoCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoDto>();

            // 1. Obtener el pago
            var pago = await _PagoRepository.GetByAsync(
                p => p.IdPago == request.CompletarDto.IdPago,
                p => p.IdEstadoPagoNavigation,
                p => p.IdMetodoPagoNavigation!,
                p => p.IdReservaNavigation!
            );

            if (pago == null)
            {
                response.AddErrorResult("El pago no existe.");
                return response;
            }

            // 2. Validar que el método de pago sea EFECTIVO
            if (pago.IdMetodoPagoNavigation?.Codigo != Constants.METODO_PAGO.Efectivo)
            {
                response.AddErrorResult($"Solo se pueden completar pagos en efectivo. Método actual: {pago.IdMetodoPagoNavigation?.Nombre}");
                return response;
            }

            // 3. Validar que el pago esté en estado PARCIAL
            if (pago.IdEstadoPagoNavigation.Codigo != Constants.ESTADO_PAGO.Parcial)
            {
                response.AddErrorResult($"El pago debe estar en estado Parcial para completarlo. Estado actual: {pago.IdEstadoPagoNavigation.Nombre}");
                return response;
            }

            // 4. Validar que el monto coincida con lo pendiente
            if (request.CompletarDto.MontoRestante != pago.MontoPendiente)
            {
                response.AddErrorResult($"El monto restante no coincide. Se esperaba: S/ {pago.MontoPendiente:F2}, Se recibió: S/ {request.CompletarDto.MontoRestante:F2}");
                return response;
            }

            // 5. Actualizar el pago
            pago.MontoAdelanto += request.CompletarDto.MontoRestante;
            pago.MontoPendiente = 0;

            // 6. Actualizar número de referencia si se proporciona
            if (!string.IsNullOrWhiteSpace(request.CompletarDto.NumeroRecibo))
            {
                pago.NumeroReferencia = request.CompletarDto.NumeroRecibo;
            }

            // 7. Cambiar estado a PAGADO
            var estadoPagado = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pagado));

            if (estadoPagado == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago 'Pagado' no encontrado.");
                return response;
            }

            pago.IdEstadoPago = estadoPagado.IdEstadoPago;

            await _PagoRepository.UpdateAsync(pago);
            await _PagoRepository.SaveAsync();

            // 8. Confirmar la reserva asociada si aún no está confirmada
            if (pago.IdReserva.HasValue && pago.IdReservaNavigation != null)
            {
                if (pago.IdReservaNavigation.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Confirmado)
                {
                    var estadoConfirmado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                        x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Confirmado));

                    if (estadoConfirmado != null)
                    {
                        var reserva = pago.IdReservaNavigation;
                        reserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;

                        await _ReservaRepository.UpdateAsync(reserva);
                        await _ReservaRepository.SaveAsync();
                    }
                }
            }

            // 9. Preparar respuesta
            var pagoDto = _mapper?.Map<GetPagoDto>(pago);
            if (pagoDto != null)
            {
                response.UpdateData(pagoDto);
            }

            response.AddOkResult($"Pago completado exitosamente. Total pagado: S/ {pago.MontoAdelanto:F2}. Reserva confirmada.");

            return response;
        }
    }
}
