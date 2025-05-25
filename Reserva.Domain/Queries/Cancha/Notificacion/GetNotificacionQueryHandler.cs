using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Notificacion;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class GetNotificacionQueryHandler : QueryHandlerBase<GetNotificacionQuery, GetNotificacionDto>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public GetNotificacionQueryHandler(
            IMapper mapper,
            GetNotificacionQueryValidator validator,
            IRepository<Entity.Models.Notificacion> NotificacionRepository
        ) : base(mapper, validator)
        {
            _NotificacionRepository = NotificacionRepository;
        }

        protected override async Task<ResponseDto<GetNotificacionDto>> HandleQuery(GetNotificacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetNotificacionDto>();
            var Notificacion = await _NotificacionRepository.GetByAsync(x => x.IdNotificacion == request.Id);
            var NotificacionDto = _mapper?.Map<GetNotificacionDto>(Notificacion);

            if (Notificacion != null && NotificacionDto != null)
            {
                response.UpdateData(NotificacionDto);
            }

            return await Task.FromResult(response);
        }
    }
}
