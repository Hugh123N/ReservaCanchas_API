using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Proveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class SelectProveedorQueryHandler : SearchQueryHandlerBase<SelectProveedorQuery, SelectProveedorFilterDto, SelectProveedorDto>
    {
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;

        public SelectProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Proveedor> ProveedorRepository
        ) : base(mapper)
        {
            _ProveedorRepository = ProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectProveedorDto>>> HandleQuery(SelectProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectProveedorDto>>();

            Expression<Func<Entity.Models.Proveedor, bool>> filter = x => true;

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

            if (filters?.IdProveedor.HasValue == true)
                filter = filter.And(x => x.IdProveedor == filters.IdProveedor);

            var sorts = new List<SortExpression<Entity.Models.Proveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Proveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Proveedors = await _ProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ProveedorDtos = _mapper?.Map<IEnumerable<SelectProveedorDto>>(Proveedors.Items);

            var searchResult = new SearchResultDto<SelectProveedorDto>(
                ProveedorDtos ?? new List<SelectProveedorDto>(),
                Proveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
