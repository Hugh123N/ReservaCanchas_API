using AutoMapper;
using MediatR;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Queries.Dbo.HorarioCancha;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Dbo.TipoDeporte;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class SearchCanchaQueryHandler : SearchQueryHandlerBase<SearchCanchaQuery, SearchCanchaFilterDto, SearchCanchaDto>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public SearchCanchaQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository
        ) : base(mapper, mediator)
        {
            _CanchaRepository = CanchaRepository;
            _TipoDeporteRepository = TipoDeporteRepository;
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

            if (filters?.IdTipoDeporte.HasValue == true)
                filter = filter.And(x => x.TipoDeporteCancha.Any(x => x.Activo && x.IdTipoDeporte == filters.IdTipoDeporte));

            if (filters?.IdEstadoCancha.HasValue == true)
                filter = filter.And(x => x.IdEstadoCancha == filters.IdEstadoCancha);

            if (filters?.Area != null)
            {
                filter = filter.And(x =>
                    x.Latitud.HasValue &&
                    x.Longitud.HasValue &&
                    x.Latitud.Value >= filters.Area.Sur &&
                    x.Latitud.Value <= filters.Area.Norte &&
                    x.Longitud.Value >= filters.Area.Oeste &&
                    x.Longitud.Value <= filters.Area.Este
                );
            }

            if (filters?.Fecha.HasValue == true)
            {
                var fecha = filters.Fecha.Value;
                var diaSemana = fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;

                if (fecha.Date > DateTime.Now.Date)
                {
                    filter = filter.And(x =>
                        x.HorarioCancha.Any(hc => hc.Activo && hc.IdDiaSemana == diaSemana)
                    );
                }
                else 
                {
                    var horaActual = TimeOnly.FromDateTime(DateTime.Now);
                    filter = filter.And(x => x.HorarioCancha.Any(hc => hc.Activo
                            && hc.IdDiaSemana == diaSemana
                            && hc.IdHoraInicioNavigation.Hora1 >= horaActual
                        )
                    );
                }
            }

            // Filtro por favoritos del usuario
            if (filters?.SoloFavoritos == true && filters?.IdUsuario.HasValue == true)
            {
                filter = filter.And(x => x.CanchaFavorita.Any(cf =>
                    cf.IdUsuario == filters.IdUsuario.Value &&
                    cf.Activo == true
                ));
            }

            // Filtro por proveedor
            if (filters?.IdProveedor.HasValue == true)
                filter = filter.And(x => x.IdProveedor == filters.IdProveedor);

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
                x => x.TipoDeporteCancha,
                x => x.ImagenCancha.Where(i => i.EsPrincipal == true),
                x => x.IdEstadoCanchaNavigation,
                x => x.CanchaFavorita.Where(x => x.Activo),
                x => x.CodigoUbigeoNavigation!
            );

            var CanchaDtos = _mapper?.Map<IEnumerable<SearchCanchaDto>>(Canchas.Items);

            var tipoDeportes = await _TipoDeporteRepository.FindByAsNoTrackingAsync(x => x.Activo);

            foreach (var it in CanchaDtos) {
                var tipoDeportesDto = new List<GetTipoDeporteDto>();
                var canchaEntity = Canchas.Items.FirstOrDefault(x => x.IdCancha == it.IdCancha);

                var idsTipoDeportes = canchaEntity.TipoDeporteCancha.Where(x => x.Activo).Select(x => x.IdTipoDeporte).ToList();

                if (idsTipoDeportes.Count() > 0) {
                    var tipoDeportesFilter = tipoDeportes.Where(x => idsTipoDeportes.Contains(x.IdTipoDeporte));
                    tipoDeportesDto = _mapper?.Map<List<GetTipoDeporteDto>>(tipoDeportesFilter);
                }

                it.TipoDeportes = tipoDeportesDto;
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
