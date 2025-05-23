using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class SearchZonaQueryHandler : SearchQueryHandlerBase<SearchZonaQuery, SearchZonaFilterDto, SearchZonaDto>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public SearchZonaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Zona> ZonaRepository
        ) : base(mapper)
        {
            _ZonaRepository = ZonaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchZonaDto>>> HandleQuery(SearchZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchZonaDto>>();

            Expression<Func<Entity.Models.Zona, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.Zona>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Zona>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Zonas = await _ZonaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ZonaDtos = _mapper?.Map<IEnumerable<SearchZonaDto>>(Zonas.Items);

            var searchResult = new SearchResultDto<SearchZonaDto>(
                ZonaDtos ?? new List<SearchZonaDto>(),
                Zonas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
