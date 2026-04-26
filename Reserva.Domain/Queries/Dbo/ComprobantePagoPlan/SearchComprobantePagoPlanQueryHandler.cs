using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class SearchComprobantePagoPlanQueryHandler : SearchQueryHandlerBase<SearchComprobantePagoPlanQuery, SearchComprobantePagoPlanFilterDto, SearchComprobantePagoPlanDto>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public SearchComprobantePagoPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository
        ) : base(mapper)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>> HandleQuery(SearchComprobantePagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>();

            Expression<Func<Entity.ComprobantePagoPlan, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.ComprobantePagoPlan>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.ComprobantePagoPlan>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var ComprobantePagoPlans = await _ComprobantePagoPlanRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ComprobantePagoPlanDtos = _mapper?.Map<IEnumerable<SearchComprobantePagoPlanDto>>(ComprobantePagoPlans.Items);

            var searchResult = new SearchResultDto<SearchComprobantePagoPlanDto>(
                ComprobantePagoPlanDtos ?? new List<SearchComprobantePagoPlanDto>(),
                ComprobantePagoPlans.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
