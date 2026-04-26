using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class SearchProveedorPlanQueryHandler : SearchQueryHandlerBase<SearchProveedorPlanQuery, SearchProveedorPlanFilterDto, SearchProveedorPlanDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public SearchProveedorPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ProveedorPlan> ProveedorPlanRepository
        ) : base(mapper)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchProveedorPlanDto>>> HandleQuery(SearchProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchProveedorPlanDto>>();

            Expression<Func<Entity.ProveedorPlan, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.ProveedorPlan>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.ProveedorPlan>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var ProveedorPlans = await _ProveedorPlanRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ProveedorPlanDtos = _mapper?.Map<IEnumerable<SearchProveedorPlanDto>>(ProveedorPlans.Items);

            var searchResult = new SearchResultDto<SearchProveedorPlanDto>(
                ProveedorPlanDtos ?? new List<SearchProveedorPlanDto>(),
                ProveedorPlans.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
