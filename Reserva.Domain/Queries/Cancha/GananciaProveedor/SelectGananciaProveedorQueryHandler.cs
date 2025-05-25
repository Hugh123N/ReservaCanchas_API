using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class SelectGananciaProveedorQueryHandler : SearchQueryHandlerBase<SelectGananciaProveedorQuery, SelectGananciaProveedorFilterDto, SelectGananciaProveedorDto>
    {
        private readonly IRepository<Entity.Models.GananciaProveedor> _GananciaProveedorRepository;

        public SelectGananciaProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.GananciaProveedor> GananciaProveedorRepository
        ) : base(mapper)
        {
            _GananciaProveedorRepository = GananciaProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectGananciaProveedorDto>>> HandleQuery(SelectGananciaProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectGananciaProveedorDto>>();

            Expression<Func<Entity.Models.GananciaProveedor, bool>> filter = x => true;

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

            if (filters?.IdGananciaProveedor.HasValue == true)
                filter = filter.And(x => x.IdGananciaProveedor == filters.IdGananciaProveedor);

            var sorts = new List<SortExpression<Entity.Models.GananciaProveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.GananciaProveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var GananciaProveedors = await _GananciaProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var GananciaProveedorDtos = _mapper?.Map<IEnumerable<SelectGananciaProveedorDto>>(GananciaProveedors.Items);

            var searchResult = new SearchResultDto<SelectGananciaProveedorDto>(
                GananciaProveedorDtos ?? new List<SelectGananciaProveedorDto>(),
                GananciaProveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
