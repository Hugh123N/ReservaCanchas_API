using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class SearchMetodoPagoQueryHandler : SearchQueryHandlerBase<SearchMetodoPagoQuery, SearchMetodoPagoFilterDto, SearchMetodoPagoDto>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public SearchMetodoPagoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.MetodoPago> MetodoPagoRepository
        ) : base(mapper)
        {
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchMetodoPagoDto>>> HandleQuery(SearchMetodoPagoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchMetodoPagoDto>>();

            Expression<Func<Entity.Models.MetodoPago, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.MetodoPago>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.MetodoPago>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var MetodoPagos = await _MetodoPagoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var MetodoPagoDtos = _mapper?.Map<IEnumerable<SearchMetodoPagoDto>>(MetodoPagos.Items);

            var searchResult = new SearchResultDto<SearchMetodoPagoDto>(
                MetodoPagoDtos ?? new List<SearchMetodoPagoDto>(),
                MetodoPagos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
