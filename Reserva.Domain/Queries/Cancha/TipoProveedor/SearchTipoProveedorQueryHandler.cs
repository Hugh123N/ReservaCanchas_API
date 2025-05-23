using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class SearchTipoProveedorQueryHandler : SearchQueryHandlerBase<SearchTipoProveedorQuery, SearchTipoProveedorFilterDto, SearchTipoProveedorDto>
    {
        private readonly IRepository<Entity.Models.TipoProveedor> _TipoProveedorRepository;

        public SearchTipoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.TipoProveedor> TipoProveedorRepository
        ) : base(mapper)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchTipoProveedorDto>>> HandleQuery(SearchTipoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchTipoProveedorDto>>();

            Expression<Func<Entity.Models.TipoProveedor, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.TipoProveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.TipoProveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var TipoProveedors = await _TipoProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var TipoProveedorDtos = _mapper?.Map<IEnumerable<SearchTipoProveedorDto>>(TipoProveedors.Items);

            var searchResult = new SearchResultDto<SearchTipoProveedorDto>(
                TipoProveedorDtos ?? new List<SearchTipoProveedorDto>(),
                TipoProveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
