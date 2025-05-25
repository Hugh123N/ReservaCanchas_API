using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Notificacion;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class UpdateNotificacionCommandHandler : CommandHandlerBase<UpdateNotificacionCommand, GetNotificacionDto>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public UpdateNotificacionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateNotificacionCommandValidator validator,
            IRepository<Entity.Models.Notificacion> NotificacionRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _NotificacionRepository = NotificacionRepository;
        }

        public override async Task<ResponseDto<GetNotificacionDto>> HandleCommand(UpdateNotificacionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetNotificacionDto>();
            var Notificacion = await _NotificacionRepository.GetByAsync(x => x.IdNotificacion == request.UpdateDto.IdNotificacion);

            if (Notificacion != null)
            {
                _mapper?.Map(request.UpdateDto, Notificacion);
                await _NotificacionRepository.UpdateAsync(Notificacion);
                await _NotificacionRepository.SaveAsync();
            }

            var NotificacionDto = _mapper?.Map<GetNotificacionDto>(Notificacion);
            if (NotificacionDto != null) response.UpdateData(NotificacionDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
