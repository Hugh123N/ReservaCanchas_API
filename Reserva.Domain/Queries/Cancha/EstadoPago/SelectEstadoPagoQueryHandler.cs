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
    public class SelectEstadoPagoQueryHandler : SearchQueryHandlerBase<SelectEstadoPagoQuery, SelectEstadoPagoFilterDto, SelectEstadoPagoDto>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public SelectEstadoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(mapper)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectEstadoPagoDto>>> HandleQuery(SelectEstadoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectEstadoPagoDto>>();

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

            if (filters?.IdEstadoPago.HasValue == true)
                filter = filter.And(x => x.IdEstadoPago == filters.IdEstadoPago);

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

            var EstadoPagoDtos = _mapper?.Map<IEnumerable<SelectEstadoPagoDto>>(EstadoPagos.Items);

            var searchResult = new SearchResultDto<SelectEstadoPagoDto>(
                EstadoPagoDtos ?? new List<SelectEstadoPagoDto>(),
                EstadoPagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
