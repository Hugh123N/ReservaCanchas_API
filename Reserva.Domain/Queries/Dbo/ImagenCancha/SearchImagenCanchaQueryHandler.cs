using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.ImagenCancha
{
    public class SearchImagenCanchaQueryHandler : SearchQueryHandlerBase<SearchImagenCanchaQuery, SearchImagenCanchaFilterDto, SearchImagenCanchaDto>
    {
        private readonly IRepository<Entity.ImagenCancha> _ImagenCanchaRepository;

        public SearchImagenCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.ImagenCancha> ImagenCanchaRepository
        ) : base(mapper)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchImagenCanchaDto>>> HandleQuery(SearchImagenCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchImagenCanchaDto>>();

            Expression<Func<Entity.ImagenCancha, bool>> filter = x => true;

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

            var sorts = new List<SortExpression<Entity.ImagenCancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.ImagenCancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var ImagenCanchas = await _ImagenCanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ImagenCanchaDtos = _mapper?.Map<IEnumerable<SearchImagenCanchaDto>>(ImagenCanchas.Items);

            var searchResult = new SearchResultDto<SearchImagenCanchaDto>(
                ImagenCanchaDtos ?? new List<SearchImagenCanchaDto>(),
                ImagenCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
