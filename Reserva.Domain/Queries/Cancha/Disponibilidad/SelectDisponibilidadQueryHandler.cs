using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Entity.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class SelectDisponibilidadQueryHandler : SearchQueryHandlerBase<SelectDisponibilidadQuery, SelectDisponibilidadFilterDto, SelectDisponibilidadDto>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public SelectDisponibilidadQueryHandler(
            IMapper mapper,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository
        ) : base(mapper)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SelectDisponibilidadDto>>> HandleQuery(SelectDisponibilidadQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SelectDisponibilidadDto>>();

            Expression<Func<Entity.Models.Disponibilidad, bool>> filter = x => true;

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

            if (filters?.IdDisponibilidad.HasValue == true)
                filter = filter.And(x => x.IdDisponibilidad == filters.IdDisponibilidad);

            var sorts = new List<SortExpression<Entity.Models.Disponibilidad>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Models.Disponibilidad>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Disponibilidads = await _DisponibilidadRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var DisponibilidadDtos = _mapper?.Map<IEnumerable<SelectDisponibilidadDto>>(Disponibilidads.Items);

            var searchResult = new SearchResultDto<SelectDisponibilidadDto>(
                DisponibilidadDtos ?? new List<SelectDisponibilidadDto>(),
                Disponibilidads.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
