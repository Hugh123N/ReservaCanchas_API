using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reclamaciones;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using Reserva.Repository.Security;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Reclamacion
{
    public class SearchReclamacionQueryHandler : SearchQueryHandlerBase<SearchReclamacionQuery, SearchReclamacionFilterDto, SearchReclamacionDto>
    {
        private readonly IRepository<Entity.Reclamacion> _ReclamacionRepository;
        private readonly IUserIdentity _userIdentity;
        private static readonly HashSet<string> SortProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Entity.Reclamacion.IdReclamacion), nameof(Entity.Reclamacion.NumeroHoja),
            nameof(Entity.Reclamacion.FechaRegistro), nameof(Entity.Reclamacion.Nombres),
            nameof(Entity.Reclamacion.Tipo), nameof(Entity.Reclamacion.Estado)
        };

        public SearchReclamacionQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reclamacion> ReclamacionRepository,
            IUserIdentity userIdentity
        ) : base(mapper)
        {
            _ReclamacionRepository = ReclamacionRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchReclamacionDto>>> HandleQuery(SearchReclamacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchReclamacionDto>>();
            if (!ReclamacionReglas.EsAdmin(_userIdentity))
            {
                response.AddErrorResult("Acceso restringido al administrador.");
                return response;
            }

            var searchParams = request.SearchParams;
            var filters = searchParams.Filter ?? new SearchReclamacionFilterDto();
            var page = searchParams.Page!;
            if (page.Page > 100000 || page.PageSize > 100 || filters.Texto?.Length > 150 ||
                (!string.IsNullOrEmpty(filters.Estado) && filters.Estado is not ("PENDIENTE" or "EN_ATENCION" or "RESPONDIDA")))
            {
                response.AddErrorResult("Los filtros de consulta no son válidos.");
                return response;
            }

            Expression<Func<Entity.Reclamacion, bool>> filter = x => x.Activo;
            if (!string.IsNullOrEmpty(filters.Estado))
                filter = filter.And(x => x.Estado == filters.Estado);

            var texto = filters.Texto?.Trim();
            if (!string.IsNullOrEmpty(texto))
                filter = filter.And(x => (x.NumeroHoja != null && x.NumeroHoja.Contains(texto)) || x.Nombres.Contains(texto) || x.NroDoc.Contains(texto));

            var sorts = new List<SortExpression<Entity.Reclamacion>>();
            if (searchParams.Sort != null)
            {
                foreach (var sort in searchParams.Sort)
                {
                    if (string.IsNullOrEmpty(sort.Property) || !SortProperties.Contains(sort.Property))
                    {
                        response.AddErrorResult("El campo de ordenamiento no es válido.");
                        return response;
                    }

                    sorts.Add(IQueryableExtensions.GetSortExpression<Entity.Reclamacion>(sort.Direction, sort.Property));
                }
            }

            if (sorts.Count == 0)
                sorts.Add(new SortExpression<Entity.Reclamacion> { Direction = SortDirection.Desc, Property = x => x.FechaRegistro });

            sorts.Add(new SortExpression<Entity.Reclamacion> { Direction = SortDirection.Desc, Property = x => x.IdReclamacion });

            var reclamaciones = await _ReclamacionRepository.SearchByAsNoTrackingAsync(page.Page, page.PageSize, sorts, filter);
            var reclamacionDtos = _mapper!.Map<IEnumerable<SearchReclamacionDto>>(reclamaciones.Items);
            var searchResult = new SearchResultDto<SearchReclamacionDto>(reclamacionDtos, reclamaciones.Total, searchParams);

            response.UpdateData(searchResult);
            return response;
        }
    }
}
