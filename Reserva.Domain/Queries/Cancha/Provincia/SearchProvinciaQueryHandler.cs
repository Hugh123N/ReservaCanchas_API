using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class SearchProvinciaQueryHandler : SearchQueryHandlerBase<SearchProvinciaQuery, SearchProvinciaFilterDto, SearchProvinciaDto>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public SearchProvinciaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Provincia> ProvinciaRepository
        ) : base(mapper)
        {
            _ProvinciaRepository = ProvinciaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchProvinciaDto>>> HandleQuery(SearchProvinciaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchProvinciaDto>>();

            Expression<Func<Entity.Models.Provincia, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.Provincia>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Provincia>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Provincias = await _ProvinciaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ProvinciaDtos = _mapper?.Map<IEnumerable<SearchProvinciaDto>>(Provincias.Items);

            var searchResult = new SearchResultDto<SearchProvinciaDto>(
                ProvinciaDtos ?? new List<SearchProvinciaDto>(),
                Provincias.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
