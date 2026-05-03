using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Notificacion;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class CreateNotificacionesMassiveCommandHandler : CommandHandlerBase<CreateNotificacionesMassiveCommand>
    {
        private readonly IRepository<Entity.Notificacion> _notificacionRepository;

        public CreateNotificacionesMassiveCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.Notificacion> notificacionRepository
        ) : base(unitOfWork, mapper, mediator)
        {
            _notificacionRepository = notificacionRepository;
        }

        public override async Task<ResponseDto> HandleCommand(CreateNotificacionesMassiveCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            if (request.Notificaciones == null || !request.Notificaciones.Any())
            {
                response.AddOkResult("No hay notificaciones para crear");
                return response;
            }

            var entities = _mapper?.Map<List<Entity.Notificacion>>(request.Notificaciones) ?? new List<Entity.Notificacion>();

            await _notificacionRepository.AddAsync(entities.ToArray());
            await _notificacionRepository.SaveAsync();

            response.AddOkResult($"Se crearon {entities.Count} notificaciones");
            return response;
        }
    }
}
