using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Ubigeo;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Ubigeo
{
    public class SelectUbigeoQueryHandler : SearchQueryHandlerBase<SelectUbigeoQuery, SelectUbigeoFilterDto, SelectUbigeoDto>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public SelectUbigeoQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Ubigeo> UbigeoRepository
        ) : base(mapper)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectUbigeoDto>>> HandleQuery(SelectUbigeoQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectUbigeoDto>>();

            Expression<Func<Entity.Models.Ubigeo, bool>> filter = x => true;

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

            if (!string.IsNullOrEmpty(filters?.IdUbigeo))
                filter = filter.And(x => x.CodigoUbigeo == filters.IdUbigeo);

            var sorts = new List<SortExpression<Entity.Models.Ubigeo>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Ubigeo>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Ubigeos = await _UbigeoRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var UbigeoDtos = _mapper?.Map<IEnumerable<SelectUbigeoDto>>(Ubigeos.Items);

            var searchResult = new SearchResultDto<SelectUbigeoDto>(
                UbigeoDtos ?? [],
                Ubigeos.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
