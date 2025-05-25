using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class SearchEstadoReservaQueryHandler : SearchQueryHandlerBase<SearchEstadoReservaQuery, SearchEstadoReservaFilterDto, SearchEstadoReservaDto>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _EstadoReservaRepository;

        public SearchEstadoReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoReserva> EstadoReservaRepository
        ) : base(mapper)
        {
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchEstadoReservaDto>>> HandleQuery(SearchEstadoReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchEstadoReservaDto>>();

            Expression<Func<Entity.Models.EstadoReserva, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.EstadoReserva>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.EstadoReserva>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var EstadoReservas = await _EstadoReservaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var EstadoReservaDtos = _mapper?.Map<IEnumerable<SearchEstadoReservaDto>>(EstadoReservas.Items);

            var searchResult = new SearchResultDto<SearchEstadoReservaDto>(
                EstadoReservaDtos ?? new List<SearchEstadoReservaDto>(),
                EstadoReservas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
