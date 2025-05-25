using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class SearchComisionQueryHandler : SearchQueryHandlerBase<SearchComisionQuery, SearchComisionFilterDto, SearchComisionDto>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public SearchComisionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Comision> ComisionRepository
        ) : base(mapper)
        {
            _ComisionRepository = ComisionRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchComisionDto>>> HandleQuery(SearchComisionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchComisionDto>>();

            Expression<Func<Entity.Models.Comision, bool>> filter = x => true;

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
            filter = filter.And(x => x.Activo == true);

            var sorts = new List<SortExpression<Entity.Models.Comision>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Comision>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Comisions = await _ComisionRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ComisionDtos = _mapper?.Map<IEnumerable<SearchComisionDto>>(Comisions.Items);

            var searchResult = new SearchResultDto<SearchComisionDto>(
                ComisionDtos ?? new List<SearchComisionDto>(),
                Comisions.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
