using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Hora;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class SearchHoraQueryHandler : SearchQueryHandlerBase<SearchHoraQuery, SearchHoraFilterDto, SearchHoraDto>
    {
        private readonly IRepository<Entity.Hora> _HoraRepository;

        public SearchHoraQueryHandler(
            IMapper mapper,
            IRepository<Entity.Hora> HoraRepository
        ) : base(mapper)
        {
            _HoraRepository = HoraRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchHoraDto>>> HandleQuery(SearchHoraQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchHoraDto>>();

            Expression<Func<Entity.Hora, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Hora>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Hora>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Horas = await _HoraRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var HoraDtos = _mapper?.Map<IEnumerable<SearchHoraDto>>(Horas.Items);

            var searchResult = new SearchResultDto<SearchHoraDto>(
                HoraDtos ?? new List<SearchHoraDto>(),
                Horas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
