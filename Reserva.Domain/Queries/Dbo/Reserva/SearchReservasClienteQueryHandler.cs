using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Helpers;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Extensions;
using System.Linq.Expressions;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Handler para buscar reservas del cliente con paginación y filtros
    /// </summary>
    public class SearchReservasClienteQueryHandler : SearchQueryHandlerBase<SearchReservasClienteQuery, SearchReservaClienteFilterDto, ReservaClienteDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;

        public SearchReservasClienteQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository
        ) : base(mapper)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _estadoPagoRepository = estadoPagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<ReservaClienteDto>>> HandleQuery(
            SearchReservasClienteQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<ReservaClienteDto>>();

            var estadosPago = await _estadoPagoRepository.FindByAsNoTrackingAsync(x => x.Activo);

            Expression<Func<Entity.Reserva, bool>> filter = x => x.IdCliente == request.IdUsuario && x.Activo;

            var filters = request.SearchParams?.Filter;

            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.CodigoEstado))
                {
                    filter = filter.And(x => x.IdEstadoReservaNavigation.Codigo == filters.CodigoEstado);
                }

                if (filters.FechaDesde.HasValue)
                {
                    filter = filter.And(x => x.FechaReserva >= filters.FechaDesde.Value);
                }

                if (filters.FechaHasta.HasValue)
                {
                    filter = filter.And(x => x.FechaReserva <= filters.FechaHasta.Value);
                }

                if (!string.IsNullOrWhiteSpace(filters.CodigoReserva))
                {
                    filter = filter.And(x => x.CodigoReserva.Contains(filters.CodigoReserva));
                }

                if (!string.IsNullOrWhiteSpace(filters.NombreCancha))
                {
                    filter = filter.And(x => x.IdCanchaNavigation.Nombre.Contains(filters.NombreCancha));
                }

                if (!string.IsNullOrWhiteSpace(filters.EstadoPago))
                {
                    filter = filter.And(x => x.Pago.Any(p => p.Activo && p.IdEstadoPagoNavigation.Nombre == filters.EstadoPago));
                }
            }

            var sorts = new List<SortExpression<Entity.Reserva>>();

            if (request.SearchParams?.Sort != null && request.SearchParams.Sort.Any())
            {
                foreach (var srt in request.SearchParams.Sort)
                {
                    var property = IQueryableExtensions.GetSortExpression<Entity.Reserva>(srt.Direction, srt.Property);
                    if (property != null) sorts.Add(property);
                }
            }

            var reservasPaginadas = await _reservaRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter,
                r => r.IdCanchaNavigation,
                r => r.IdEstadoReservaNavigation,
                r => r.Pago
            );

            if (!reservasPaginadas.Items.Any())
            {
                var emptyResult = new SearchResultDto<ReservaClienteDto>(
                    new List<ReservaClienteDto>(),
                    0,
                    request.SearchParams
                );
                response.UpdateData(emptyResult);
                response.AddOkResult("No se encontraron reservas.");
                return response;
            }

            var idsReservas = reservasPaginadas.Items.Select(r => r.IdReserva).ToList();

            var todosLosDetalles = await _detalleReservaRepository.FindByAsNoTrackingAsync(
                d => idsReservas.Contains(d.IdReserva) && d.Activo,
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
            );

            // Agrupar detalles por IdReserva para acceso rápido
            var detallesPorReserva = todosLosDetalles
                .GroupBy(d => d.IdReserva)
                .ToDictionary(g => g.Key, g => g.ToList());

            var reservasDtos = new List<ReservaClienteDto>();

            foreach (var r in reservasPaginadas.Items)
            {
                var horarios = new List<HorarioDisponibleDto>();

                // Obtener detalles de esta reserva desde el diccionario
                if (detallesPorReserva.TryGetValue(r.IdReserva, out var detallesReserva))
                {
                    horarios = HorarioHelper.AgruparHorariosDesdeDetalles(detallesReserva);
                }

                var estadoPago = "Desconocido";

                var estado = estadosPago!.FirstOrDefault(x => x.IdEstadoPago == r.Pago.FirstOrDefault(p => p.Activo)?.IdEstadoPago);
                if (estado != null)
                    estadoPago = estado.Descripcion;

                reservasDtos.Add(new ReservaClienteDto
                {
                    IdReserva = r.IdReserva,
                    CodigoReserva = r.CodigoReserva,
                    Fecha = r.FechaReserva,
                    Monto = r.MontoTotal,

                    // Estado
                    EstadoReserva = r.IdEstadoReservaNavigation.Nombre,
                    CodigoEstadoReserva = r.IdEstadoReservaNavigation.Codigo!,

                    // Cancha
                    IdCancha = r.IdCancha,
                    NombreCancha = r.IdCanchaNavigation.Nombre,
                    DireccionCancha = r.IdCanchaNavigation.Direccion,
                    TelefonoCancha = r.IdCanchaNavigation.TelefonoCancha,

                    // Horarios
                    Horarios = horarios,

                    // Pago
                    EstadoPago = estadoPago,
                    MontoAdelanto = r.Pago.FirstOrDefault(p => p.Activo)?.MontoAdelanto ?? 0,
                    MontoPendiente = r.Pago.FirstOrDefault(p => p.Activo)?.MontoPendiente ?? 0,
                    NumeroRecibo = r.Pago.FirstOrDefault(p => p.Activo)?.NumeroReferencia,

                    // Fechas
                    FechaExpiracionPreReserva = r.FechaExpiracionPreReserva,
                    FechaCreacion = r.CreateDate
                });
            }

            var searchResult = new SearchResultDto<ReservaClienteDto>(
                reservasDtos,
                reservasPaginadas.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);
            response.AddOkResult($"Se encontraron {reservasPaginadas.Total} reservas.");
            
            return response;
        }
    }
}
