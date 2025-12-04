using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class SearchHorarioCanchaQueryHandler : SearchQueryHandlerBase<SearchHorarioCanchaQuery, SearchHorarioCanchaFilterDto, SearchHorarioCanchaDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public SearchHorarioCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository
        ) : base(mapper)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchHorarioCanchaDto>>> HandleQuery(SearchHorarioCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchHorarioCanchaDto>>();

            Expression<Func<Entity.HorarioCancha, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.HorarioCancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.HorarioCancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var HorarioCanchas = await _HorarioCanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var HorarioCanchaDtos = _mapper?.Map<IEnumerable<SearchHorarioCanchaDto>>(HorarioCanchas.Items);

            var searchResult = new SearchResultDto<SearchHorarioCanchaDto>(
                HorarioCanchaDtos ?? new List<SearchHorarioCanchaDto>(),
                HorarioCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
