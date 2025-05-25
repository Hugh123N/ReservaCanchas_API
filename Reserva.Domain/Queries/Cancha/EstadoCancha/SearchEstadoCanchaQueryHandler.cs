using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class SearchEstadoCanchaQueryHandler : SearchQueryHandlerBase<SearchEstadoCanchaQuery, SearchEstadoCanchaFilterDto, SearchEstadoCanchaDto>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public SearchEstadoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(mapper)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchEstadoCanchaDto>>> HandleQuery(SearchEstadoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchEstadoCanchaDto>>();

            Expression<Func<Entity.Models.EstadoCancha, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.EstadoCancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.EstadoCancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var EstadoCanchas = await _EstadoCanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var EstadoCanchaDtos = _mapper?.Map<IEnumerable<SearchEstadoCanchaDto>>(EstadoCanchas.Items);

            var searchResult = new SearchResultDto<SearchEstadoCanchaDto>(
                EstadoCanchaDtos ?? new List<SearchEstadoCanchaDto>(),
                EstadoCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
