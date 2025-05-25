using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class SearchReservaQueryHandler : SearchQueryHandlerBase<SearchReservaQuery, SearchReservaFilterDto, SearchReservaDto>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public SearchReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Reserva> ReservaRepository
        ) : base(mapper)
        {
            _ReservaRepository = ReservaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchReservaDto>>> HandleQuery(SearchReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchReservaDto>>();

            Expression<Func<Entity.Models.Reserva, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.Reserva>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Reserva>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Reservas = await _ReservaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ReservaDtos = _mapper?.Map<IEnumerable<SearchReservaDto>>(Reservas.Items);

            var searchResult = new SearchResultDto<SearchReservaDto>(
                ReservaDtos ?? new List<SearchReservaDto>(),
                Reservas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
