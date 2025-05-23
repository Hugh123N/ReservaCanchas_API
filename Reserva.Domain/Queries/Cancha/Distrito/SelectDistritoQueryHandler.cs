using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class SelectDistritoQueryHandler : SearchQueryHandlerBase<SelectDistritoQuery, SelectDistritoFilterDto, SelectDistritoDto>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public SelectDistritoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Distrito> DistritoRepository
        ) : base(mapper)
        {
            _DistritoRepository = DistritoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectDistritoDto>>> HandleQuery(SelectDistritoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectDistritoDto>>();

            Expression<Func<Entity.Models.Distrito, bool>> filter = x => true;

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

            if (filters?.IdDistrito.HasValue == true)
                filter = filter.And(x => x.IdDistrito == filters.IdDistrito);

            var sorts = new List<SortExpression<Entity.Models.Distrito>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Distrito>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Distritos = await _DistritoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DistritoDtos = _mapper?.Map<IEnumerable<SelectDistritoDto>>(Distritos.Items);

            var searchResult = new SearchResultDto<SelectDistritoDto>(
                DistritoDtos ?? new List<SelectDistritoDto>(),
                Distritos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
