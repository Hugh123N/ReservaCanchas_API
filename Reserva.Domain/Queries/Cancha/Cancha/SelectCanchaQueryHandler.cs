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
    public class SelectCanchaQueryHandler : SearchQueryHandlerBase<SelectCanchaQuery, SelectCanchaFilterDto, SelectCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public SelectCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(mapper)
        {
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> HandleQuery(SelectCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectCanchaDto>>();

            Expression<Func<Entity.Models.Cancha, bool>> filter = x => true;

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

            if (filters?.IdCancha.HasValue == true)
                filter = filter.And(x => x.IdCancha == filters.IdCancha);

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
                filter
            );

            var CanchaDtos = _mapper?.Map<IEnumerable<SelectCanchaDto>>(Canchas.Items);

            var searchResult = new SearchResultDto<SelectCanchaDto>(
                CanchaDtos ?? new List<SelectCanchaDto>(),
                Canchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
