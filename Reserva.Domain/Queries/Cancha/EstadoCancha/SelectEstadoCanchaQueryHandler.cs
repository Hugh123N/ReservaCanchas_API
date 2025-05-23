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
    public class SelectEstadoCanchaQueryHandler : SearchQueryHandlerBase<SelectEstadoCanchaQuery, SelectEstadoCanchaFilterDto, SelectEstadoCanchaDto>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public SelectEstadoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(mapper)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectEstadoCanchaDto>>> HandleQuery(SelectEstadoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectEstadoCanchaDto>>();

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

            if (filters?.IdEstadoCancha.HasValue == true)
                filter = filter.And(x => x.IdEstadoCancha == filters.IdEstadoCancha);

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

            var EstadoCanchaDtos = _mapper?.Map<IEnumerable<SelectEstadoCanchaDto>>(EstadoCanchas.Items);

            var searchResult = new SearchResultDto<SelectEstadoCanchaDto>(
                EstadoCanchaDtos ?? new List<SelectEstadoCanchaDto>(),
                EstadoCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
