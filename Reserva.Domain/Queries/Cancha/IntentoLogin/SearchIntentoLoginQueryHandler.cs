using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class SearchIntentoLoginQueryHandler : SearchQueryHandlerBase<SearchIntentoLoginQuery, SearchIntentoLoginFilterDto, SearchIntentoLoginDto>
    {
        private readonly IRepository<Entity.Models.IntentoLogin> _IntentoLoginRepository;

        public SearchIntentoLoginQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.IntentoLogin> IntentoLoginRepository
        ) : base(mapper)
        {
            _IntentoLoginRepository = IntentoLoginRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchIntentoLoginDto>>> HandleQuery(SearchIntentoLoginQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchIntentoLoginDto>>();

            Expression<Func<Entity.Models.IntentoLogin, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.Models.IntentoLogin>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.IntentoLogin>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var IntentoLogins = await _IntentoLoginRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var IntentoLoginDtos = _mapper?.Map<IEnumerable<SearchIntentoLoginDto>>(IntentoLogins.Items);

            var searchResult = new SearchResultDto<SearchIntentoLoginDto>(
                IntentoLoginDtos ?? new List<SearchIntentoLoginDto>(),
                IntentoLogins.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
