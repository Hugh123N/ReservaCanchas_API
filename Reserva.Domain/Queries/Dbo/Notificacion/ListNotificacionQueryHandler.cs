using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Notificacion;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class ListNotificacionQueryHandler : QueryHandlerBase<ListNotificacionQuery, IEnumerable<ListNotificacionDto>>
    {
        private readonly IRepository<Entity.Notificacion> _repository;

        public ListNotificacionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Notificacion> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListNotificacionDto>>> HandleQuery(ListNotificacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListNotificacionDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdNotificacion == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListNotificacionDto>>(list);

            response.UpdateData(listDtos ?? new List<ListNotificacionDto>());

            return await Task.FromResult(response);
        }
    }
}
