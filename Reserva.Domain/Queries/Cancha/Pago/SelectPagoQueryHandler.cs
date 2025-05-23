using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class SelectPagoQueryHandler : SearchQueryHandlerBase<SelectPagoQuery, SelectPagoFilterDto, SelectPagoDto>
    {
        private readonly IRepository<Entity.Models.Pago> _PagoRepository;

        public SelectPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Pago> PagoRepository
        ) : base(mapper)
        {
            _PagoRepository = PagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectPagoDto>>> HandleQuery(SelectPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectPagoDto>>();

            Expression<Func<Entity.Models.Pago, bool>> filter = x => true;

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

            if (filters?.IdPago.HasValue == true)
                filter = filter.And(x => x.IdPago == filters.IdPago);

            var sorts = new List<SortExpression<Entity.Models.Pago>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Pago>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Pagos = await _PagoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var PagoDtos = _mapper?.Map<IEnumerable<SelectPagoDto>>(Pagos.Items);

            var searchResult = new SearchResultDto<SelectPagoDto>(
                PagoDtos ?? new List<SelectPagoDto>(),
                Pagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
