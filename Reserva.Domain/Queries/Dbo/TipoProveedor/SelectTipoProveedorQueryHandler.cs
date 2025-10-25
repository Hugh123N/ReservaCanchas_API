using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class SelectTipoProveedorQueryHandler : SearchQueryHandlerBase<SelectTipoProveedorQuery, SelectTipoProveedorFilterDto, SelectTipoProveedorDto>
    {
        private readonly IRepository<Entity.TipoProveedor> _TipoProveedorRepository;

        public SelectTipoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.TipoProveedor> TipoProveedorRepository
        ) : base(mapper)
        {
            _TipoProveedorRepository = TipoProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectTipoProveedorDto>>> HandleQuery(SelectTipoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectTipoProveedorDto>>();

            Expression<Func<Entity.TipoProveedor, bool>> filter = x => true;

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

            if (filters?.IdTipoProveedor.HasValue == true)
                filter = filter.And(x => x.IdTipoProveedor == filters.IdTipoProveedor);

            var sorts = new List<SortExpression<Entity.TipoProveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.TipoProveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var TipoProveedors = await _TipoProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var TipoProveedorDtos = _mapper?.Map<IEnumerable<SelectTipoProveedorDto>>(TipoProveedors.Items);

            var searchResult = new SearchResultDto<SelectTipoProveedorDto>(
                TipoProveedorDtos ?? new List<SelectTipoProveedorDto>(),
                TipoProveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
