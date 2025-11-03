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

        public LiberarReservaOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LiberarReservaOperadorCommandValidator validator,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(
            LiberarReservaOperadorCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            var reserva = await _ReservaRepository.GetByAsync(
                r => r.IdReserva == request.LiberarDto.IdReserva && r.Activo,
                r => r.IdEstadoReservaNavigation
            );

            if (reserva == null)
            {
                response.AddErrorResult("La reserva no existe o ha sido eliminada.");
                return response;
            }

            if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente)
            {
                response.AddErrorResult($"Solo las reservas pendientes pueden ser liberadas. Estado actual: {reserva.IdEstadoReservaNavigation.Nombre}");
                return response;
            }

            var estadoCancelado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Cancelado);

            if (estadoCancelado == null)
            {
                response.AddErrorResult("Error del sistema: Estado de reserva no encontrado.");
                return response;
            }

            reserva.IdEstadoReserva = estadoCancelado.IdEstadoReserva;

            reserva.FechaExpiracionPreReserva = null;

            await _ReservaRepository.UpdateAsync(reserva);
            await _ReservaRepository.SaveAsync();

            var reservaDto = _mapper?.Map<GetReservaDto>(reserva);

            response.UpdateData(reservaDto!);
            response.AddOkResult($"Reserva {reserva.CodigoReserva} liberada exitosamente. El horario está ahora disponible.");

            // TODO: Enviar notificación al cliente (opcional) informando cancelación

            return response;
        }
    }
}
