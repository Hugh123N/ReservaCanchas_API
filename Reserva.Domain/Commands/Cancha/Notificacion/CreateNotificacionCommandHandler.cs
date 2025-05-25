using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Notificacion;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class CreateNotificacionCommandHandler : CommandHandlerBase<CreateNotificacionCommand, GetNotificacionDto>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public CreateNotificacionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateNotificacionCommandValidator validator,
            IRepository<Entity.Models.Notificacion> NotificacionRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _NotificacionRepository = NotificacionRepository;
        }

        public override async Task<ResponseDto<GetNotificacionDto>> HandleCommand(CreateNotificacionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetNotificacionDto>();

            var Notificacion = _mapper?.Map<Entity.Models.Notificacion>(request.CreateDto);

            if (Notificacion != null)
            {
                await _NotificacionRepository.AddAsync(Notificacion);
                await _NotificacionRepository.SaveAsync();
            }

            var NotificacionDto = _mapper?.Map<GetNotificacionDto>(Notificacion);
            if (NotificacionDto != null) response.UpdateData(NotificacionDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}