using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class SearchDetallePagoQueryHandler : SearchQueryHandlerBase<SearchDetallePagoQuery, SearchDetallePagoFilterDto, SearchDetallePagoDto>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public SearchDetallePagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(mapper)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchDetallePagoDto>>> HandleQuery(SearchDetallePagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchDetallePagoDto>>();

            Expression<Func<Entity.Models.DetallePago, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.DetallePago>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.DetallePago>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var DetallePagos = await _DetallePagoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DetallePagoDtos = _mapper?.Map<IEnumerable<SearchDetallePagoDto>>(DetallePagos.Items);

            var searchResult = new SearchResultDto<SearchDetallePagoDto>(
                DetallePagoDtos ?? new List<SearchDetallePagoDto>(),
                DetallePagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
