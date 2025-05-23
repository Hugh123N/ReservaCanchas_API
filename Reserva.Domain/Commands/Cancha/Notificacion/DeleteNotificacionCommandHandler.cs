using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class DeleteNotificacionCommandHandler : CommandHandlerBase<DeleteNotificacionCommand>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public DeleteNotificacionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteNotificacionCommandValidator validator,
            IRepository<Entity.Models.Notificacion> NotificacionRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _NotificacionRepository = NotificacionRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteNotificacionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Notificacion = await _NotificacionRepository.GetByAsync(x => x.IdNotificacion == request.Id);

            if (Notificacion != null)
            {
                Notificacion.Activo = false;
                await _NotificacionRepository.UpdateAsync(Notificacion);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
