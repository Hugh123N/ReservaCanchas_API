using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class SelectImagenCanchaQueryHandler : SearchQueryHandlerBase<SelectImagenCanchaQuery, SelectImagenCanchaFilterDto, SelectImagenCanchaDto>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public SelectImagenCanchaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository
        ) : base(mapper)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectImagenCanchaDto>>> HandleQuery(SelectImagenCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectImagenCanchaDto>>();

            Expression<Func<Entity.Models.ImagenCancha, bool>> filter = x => true;

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

            if (filters?.IdImagenCancha.HasValue == true)
                filter = filter.And(x => x.IdImagenCancha == filters.IdImagenCancha);

            var sorts = new List<SortExpression<Entity.Models.ImagenCancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.ImagenCancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var ImagenCanchas = await _ImagenCanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var ImagenCanchaDtos = _mapper?.Map<IEnumerable<SelectImagenCanchaDto>>(ImagenCanchas.Items);

            var searchResult = new SearchResultDto<SelectImagenCanchaDto>(
                ImagenCanchaDtos ?? new List<SelectImagenCanchaDto>(),
                ImagenCanchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
