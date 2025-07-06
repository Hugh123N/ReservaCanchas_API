using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.CanchaFavorita;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.CanchaFavorita
{
    public class SearchCanchaFavoritaQueryHandler : SearchQueryHandlerBase<SearchCanchaFavoritaQuery, SearchCanchaFavoritaFilterDto, SearchCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _CanchaFavoritaRepository;

        public SearchCanchaFavoritaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.CanchaFavorita> CanchaFavoritaRepository
        ) : base(mapper)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>> HandleQuery(SearchCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchCanchaFavoritaDto>>();

            Expression<Func<Entity.Models.CanchaFavorita, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.CanchaFavorita>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.CanchaFavorita>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var CanchaFavoritas = await _CanchaFavoritaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var CanchaFavoritaDtos = _mapper?.Map<IEnumerable<SearchCanchaFavoritaDto>>(CanchaFavoritas.Items);

            var searchResult = new SearchResultDto<SearchCanchaFavoritaDto>(
                CanchaFavoritaDtos ?? new List<SearchCanchaFavoritaDto>(),
                CanchaFavoritas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
