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
    public class SelectCanchaFavoritaQueryHandler : SearchQueryHandlerBase<SelectCanchaFavoritaQuery, SelectCanchaFavoritaFilterDto, SelectCanchaFavoritaDto>
    {
        private readonly IRepository<Entity.Models.CanchaFavorita> _CanchaFavoritaRepository;

        public SelectCanchaFavoritaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.CanchaFavorita> CanchaFavoritaRepository
        ) : base(mapper)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>> HandleQuery(SelectCanchaFavoritaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectCanchaFavoritaDto>>();

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

            if (filters?.IdCanchaFavorita.HasValue == true)
                filter = filter.And(x => x.IdCancha == filters.IdCanchaFavorita);

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

            var CanchaFavoritaDtos = _mapper?.Map<IEnumerable<SelectCanchaFavoritaDto>>(CanchaFavoritas.Items);

            var searchResult = new SearchResultDto<SelectCanchaFavoritaDto>(
                CanchaFavoritaDtos ?? new List<SelectCanchaFavoritaDto>(),
                CanchaFavoritas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
