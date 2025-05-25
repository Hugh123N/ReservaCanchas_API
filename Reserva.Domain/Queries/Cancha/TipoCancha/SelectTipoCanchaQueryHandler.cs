using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class SelectTipoCanchaQueryHandler : SearchQueryHandlerBase<SelectTipoCanchaQuery, SelectTipoCanchaFilterDto, SelectTipoCanchaDto>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _TipoCanchaRepository;

        public SelectTipoCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.TipoCancha> TipoCanchaRepository
        ) : base(mapper)
        {
            _TipoCanchaRepository = TipoCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectTipoCanchaDto>>> HandleQuery(SelectTipoCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectTipoCanchaDto>>();

            Expression<Func<Entity.Models.TipoCancha, bool>> filter = x => true;

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

            if (filters?.IdTipoCancha.HasValue == true)
                filter = filter.And(x => x.IdTipoCancha == filters.IdTipoCancha);

            var sorts = new List<SortExpression<Entity.Models.TipoCancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.TipoCancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var TipoCanchas = await _TipoCanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var TipoCanchaDtos = _mapper?.Map<IEnumerable<SelectTipoCanchaDto>>(TipoCanchas.Items);

            var searchResult = new SearchResultDto<SelectTipoCanchaDto>(
                TipoCanchaDtos ?? new List<SelectTipoCanchaDto>(),
                TipoCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
