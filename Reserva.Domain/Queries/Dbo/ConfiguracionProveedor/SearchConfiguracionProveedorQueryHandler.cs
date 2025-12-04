using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class SearchConfiguracionProveedorQueryHandler : SearchQueryHandlerBase<SearchConfiguracionProveedorQuery, SearchConfiguracionProveedorFilterDto, SearchConfiguracionProveedorDto>
    {
        private readonly IRepository<Entity.ConfiguracionProveedor> _ConfiguracionProveedorRepository;

        public SearchConfiguracionProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.ConfiguracionProveedor> ConfiguracionProveedorRepository
        ) : base(mapper)
        {
            _ConfiguracionProveedorRepository = ConfiguracionProveedorRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchConfiguracionProveedorDto>>> HandleQuery(SearchConfiguracionProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchConfiguracionProveedorDto>>();

            Expression<Func<Entity.ConfiguracionProveedor, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.ConfiguracionProveedor>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.ConfiguracionProveedor>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var ConfiguracionProveedors = await _ConfiguracionProveedorRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ConfiguracionProveedorDtos = _mapper?.Map<IEnumerable<SearchConfiguracionProveedorDto>>(ConfiguracionProveedors.Items);

            var searchResult = new SearchResultDto<SearchConfiguracionProveedorDto>(
                ConfiguracionProveedorDtos ?? new List<SearchConfiguracionProveedorDto>(),
                ConfiguracionProveedors.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
