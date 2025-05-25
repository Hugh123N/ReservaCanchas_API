using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class SearchEstadoPagoQueryHandler : SearchQueryHandlerBase<SearchEstadoPagoQuery, SearchEstadoPagoFilterDto, SearchEstadoPagoDto>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public SearchEstadoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(mapper)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchEstadoPagoDto>>> HandleQuery(SearchEstadoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchEstadoPagoDto>>();

            Expression<Func<Entity.Models.EstadoPago, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.EstadoPago>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.EstadoPago>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var EstadoPagos = await _EstadoPagoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var EstadoPagoDtos = _mapper?.Map<IEnumerable<SearchEstadoPagoDto>>(EstadoPagos.Items);

            var searchResult = new SearchResultDto<SearchEstadoPagoDto>(
                EstadoPagoDtos ?? new List<SearchEstadoPagoDto>(),
                EstadoPagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
