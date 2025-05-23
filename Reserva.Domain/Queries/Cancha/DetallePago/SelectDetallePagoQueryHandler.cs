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
    public class SelectDetallePagoQueryHandler : SearchQueryHandlerBase<SelectDetallePagoQuery, SelectDetallePagoFilterDto, SelectDetallePagoDto>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public SelectDetallePagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(mapper)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectDetallePagoDto>>> HandleQuery(SelectDetallePagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectDetallePagoDto>>();

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

            if (filters?.IdDetallePago.HasValue == true)
                filter = filter.And(x => x.IdDetallePago == filters.IdDetallePago);

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

            var DetallePagoDtos = _mapper?.Map<IEnumerable<SelectDetallePagoDto>>(DetallePagos.Items);

            var searchResult = new SearchResultDto<SelectDetallePagoDto>(
                DetallePagoDtos ?? new List<SelectDetallePagoDto>(),
                DetallePagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
