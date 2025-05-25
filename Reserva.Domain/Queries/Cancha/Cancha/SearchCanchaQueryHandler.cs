using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class SearchCanchaQueryHandler : SearchQueryHandlerBase<SearchCanchaQuery, SearchCanchaFilterDto, SearchCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public SearchCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(mapper)
        {
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> HandleQuery(SearchCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchCanchaDto>>();

            Expression<Func<Entity.Models.Cancha, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;


            filter = filter.And(x => x.Activo == true);

            if (!string.IsNullOrEmpty(filters?.Nombre))
                filter = filter.And(x => x.Nombre.Contains(filters.Nombre));

            if (!string.IsNullOrEmpty(filters?.CodigoDepartamento)) 
                filter = filter.And(x => x.CodigoUbigeo!.StartsWith(filters.CodigoDepartamento));

            if (!string.IsNullOrEmpty(filters?.CodigoProvincia))
                filter = filter.And(x => x.CodigoUbigeo!.StartsWith(filters.CodigoProvincia));

            if (!string.IsNullOrEmpty(filters?.CodigoDistrito))
                filter = filter.And(x => x.CodigoUbigeo!.StartsWith(filters.CodigoDistrito));

            if (filters?.IdTipoCancha.HasValue == true)
                filter = filter.And(x => x.IdTipoCancha == filters.IdTipoCancha);

            if (filters?.IdEstadoCancha.HasValue == true)
                filter = filter.And(x => x.IdEstadoCancha == filters.IdEstadoCancha);

            var sorts = new List<SortExpression<Entity.Models.Cancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Cancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Canchas = await _CanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter,
                x => x.IdTipoCanchaNavigation,
                x => x.ImagenCanchas.Where(i => i.EsPrincipal == true)
            );

            var CanchaDtos = _mapper?.Map<IEnumerable<SearchCanchaDto>>(Canchas.Items);

            var searchResult = new SearchResultDto<SearchCanchaDto>(
                CanchaDtos ?? new List<SearchCanchaDto>(),
                Canchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
