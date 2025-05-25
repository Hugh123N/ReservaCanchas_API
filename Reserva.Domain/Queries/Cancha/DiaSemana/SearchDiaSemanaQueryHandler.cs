using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class SearchDiaSemanaQueryHandler : SearchQueryHandlerBase<SearchDiaSemanaQuery, SearchDiaSemanaFilterDto, SearchDiaSemanaDto>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public SearchDiaSemanaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DiaSemana> DiaSemanaRepository
        ) : base(mapper)
        {
            _DiaSemanaRepository = DiaSemanaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> HandleQuery(SearchDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchDiaSemanaDto>>();

            Expression<Func<Entity.Models.DiaSemana, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.DiaSemana>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.DiaSemana>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var DiaSemanas = await _DiaSemanaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DiaSemanaDtos = _mapper?.Map<IEnumerable<SearchDiaSemanaDto>>(DiaSemanas.Items);

            var searchResult = new SearchResultDto<SearchDiaSemanaDto>(
                DiaSemanaDtos ?? new List<SearchDiaSemanaDto>(),
                DiaSemanas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
