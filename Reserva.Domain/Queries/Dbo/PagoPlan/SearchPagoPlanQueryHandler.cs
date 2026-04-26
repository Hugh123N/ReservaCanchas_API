using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class SearchPagoPlanQueryHandler : SearchQueryHandlerBase<SearchPagoPlanQuery, SearchPagoPlanFilterDto, SearchPagoPlanDto>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public SearchPagoPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.PagoPlan> PagoPlanRepository
        ) : base(mapper)
        {
            _PagoPlanRepository = PagoPlanRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchPagoPlanDto>>> HandleQuery(SearchPagoPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchPagoPlanDto>>();

            Expression<Func<Entity.PagoPlan, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.PagoPlan>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.PagoPlan>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var PagoPlans = await _PagoPlanRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var PagoPlanDtos = _mapper?.Map<IEnumerable<SearchPagoPlanDto>>(PagoPlans.Items);

            var searchResult = new SearchResultDto<SearchPagoPlanDto>(
                PagoPlanDtos ?? new List<SearchPagoPlanDto>(),
                PagoPlans.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
