using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class SearchEstadoProveedorQueryHandler : SearchQueryHandlerBase<SearchEstadoProveedorQuery, SearchEstadoProveedorFilterDto, SearchEstadoProveedorDto>
    {
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;

        public SearchEstadoProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository
        ) : base(mapper)
        {
            _EstadoProveedorRepository = EstadoProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchEstadoProveedorDto>>> HandleQuery(SearchEstadoProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchEstadoProveedorDto>>();

            Expression<Func<Entity.Models.EstadoProveedor, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.EstadoProveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.EstadoProveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var EstadoProveedors = await _EstadoProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var EstadoProveedorDtos = _mapper?.Map<IEnumerable<SearchEstadoProveedorDto>>(EstadoProveedors.Items);

            var searchResult = new SearchResultDto<SearchEstadoProveedorDto>(
                EstadoProveedorDtos ?? new List<SearchEstadoProveedorDto>(),
                EstadoProveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
