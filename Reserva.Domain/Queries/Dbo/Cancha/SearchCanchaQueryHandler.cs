using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Dbo.Disponibilidad;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class SearchCanchaQueryHandler : SearchQueryHandlerBase<SearchCanchaQuery, SearchCanchaFilterDto, SearchCanchaDto>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;

        public SearchCanchaQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.Cancha> CanchaRepository
        ) : base(mapper, mediator)
        {
            _CanchaRepository = CanchaRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> HandleQuery(SearchCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchCanchaDto>>();

            Expression<Func<Entity.Cancha, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            filter = filter.And(x => x.Activo == true);

            if (!string.IsNullOrEmpty(filters?.Nombre))
                filter = filter.And(x => x.Nombre.Contains(filters.Nombre));

            if (!string.IsNullOrEmpty(filters?.CodigoUbigeo)) 
                filter = filter.And(x => x.CodigoUbigeo!.StartsWith(filters.CodigoUbigeo));

            if (filters?.IdTipoCancha.HasValue == true)
                filter = filter.And(x => x.IdTipoCancha == filters.IdTipoCancha);

            if (filters?.IdEstadoCancha.HasValue == true)
                filter = filter.And(x => x.IdEstadoCancha == filters.IdEstadoCancha);

            if (filters?.Fecha.HasValue == true)
            {
                var fecha = filters.Fecha.Value;
                var diaSemana = fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;

                filter = filter.And(x =>
                    x.Disponibilidad.Any(d => d.Activo
                        && d.IdDiaSemana == diaSemana
                        && (fecha.Date > DateTime.Now.Date || d.HoraInicio >= TimeOnly.FromDateTime(DateTime.Now))
                    )
                );
            }

            var sorts = new List<SortExpression<Entity.Cancha>>();

            if (request.SearchParams?.Sort != null)
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Cancha>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var Canchas = await _CanchaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter,
                x => x.IdTipoCanchaNavigation,
                x => x.ImagenCancha.Where(i => i.EsPrincipal == true),
                x => x.IdEstadoCanchaNavigation,
                x => x.CanchaFavorita.Where(x => x.Activo),
                x => x.CodigoUbigeoNavigation!,
                x => x.Disponibilidad.Where(d => d.Activo && d.HoraInicio >= TimeOnly.FromDateTime(DateTime.Now))
            );

            var CanchaDtos = _mapper?.Map<IEnumerable<SearchCanchaDto>>(Canchas.Items);

            foreach (var it in CanchaDtos) { 
                var gr = await _mediator?.Send(new GetCanchaByFechaQuery(DateTime.Now, it.IdCancha ?? 0), cancellationToken)!;
                it.HorariosDisponibles = gr.Data; 
            }

            var searchResult = new SearchResultDto<SearchCanchaDto>(
                CanchaDtos ?? new List<SearchCanchaDto>(),
                Canchas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
