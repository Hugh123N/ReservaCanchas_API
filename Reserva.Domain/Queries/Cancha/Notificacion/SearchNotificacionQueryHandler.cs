using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Notificacion;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Notificacion
{
    public class SearchNotificacionQueryHandler : SearchQueryHandlerBase<SearchNotificacionQuery, SearchNotificacionFilterDto, SearchNotificacionDto>
    {
        private readonly IRepository<Entity.Models.Notificacion> _NotificacionRepository;

        public SearchNotificacionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Notificacion> NotificacionRepository
        ) : base(mapper)
        {
            _NotificacionRepository = NotificacionRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchNotificacionDto>>> HandleQuery(SearchNotificacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchNotificacionDto>>();

            Expression<Func<Entity.Models.Notificacion, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            /*
            if (filters?.FechaDesde.HasValue == true || filters?.FechaHasta.HasValue == true)
            {
                if (filters?.FechaDesde.HasValue == true)
                {
                    var fechaDesde = filters.FechaDesde.GetStartDate();
                    filter = filter.And(x => x.Fecha >= fechaDesde);
                }

                if (filters?.FechaHasta.HasValue == true)
                {
                    var fechaHasta = filters.FechaHasta.GetEndDate();
                    filter = filter.And(x => x.Fecha < fechaHasta);
                }
            }
            */

            var sorts = new List<SortExpression<Entity.Models.Notificacion>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Notificacion>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Notificacions = await _NotificacionRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var NotificacionDtos = _mapper?.Map<IEnumerable<SearchNotificacionDto>>(Notificacions.Items);

            var searchResult = new SearchResultDto<SearchNotificacionDto>(
                NotificacionDtos ?? new List<SearchNotificacionDto>(),
                Notificacions.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
