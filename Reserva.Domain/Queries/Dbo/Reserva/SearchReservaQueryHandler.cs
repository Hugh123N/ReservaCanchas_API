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
    /// Handler para buscar reservas del proveedor con paginación y filtros
    /// </summary>
    public class SearchReservaQueryHandler : SearchQueryHandlerBase<SearchReservaQuery, SearchReservaFilterDto, ReservaClienteDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Pago> _pagoRepository;

        public SearchReservaQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Pago> pagoRepository
        ) : base(mapper)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _pagoRepository = pagoRepository;
        }

        protected override async Task<ResponseDto<SearchResultDto<ReservaClienteDto>>> HandleQuery(
            SearchReservaQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<ReservaClienteDto>>();

            Expression<Func<Entity.Reserva, bool>> filter = x => x.Activo;

            var filters = request.SearchParams?.Filter;

            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.CodigoEstado))
                    filter = filter.And(x => x.IdEstadoReservaNavigation.Codigo == filters.CodigoEstado);

                if (filters.FechaDesde.HasValue)
                    filter = filter.And(x => x.FechaReserva >= filters.FechaDesde.Value);
                if (filters.FechaHasta.HasValue)
                    filter = filter.And(x => x.FechaReserva <= filters.FechaHasta.Value);

                if (!string.IsNullOrWhiteSpace(filters.SearchText))
                    filter = filter.And(x =>
                        x.CodigoReserva.Contains(filters.SearchText) ||
                        x.IdClienteNavigation.FirstName.Contains(filters.SearchText) ||
                        (x.IdClienteNavigation.PhoneNumber != null && x.IdClienteNavigation.PhoneNumber.Contains(filters.SearchText))
                    );

                if (!string.IsNullOrWhiteSpace(filters.NombreCancha))
                    filter = filter.And(x => x.IdCanchaNavigation.Nombre.Contains(filters.NombreCancha));

                if (!string.IsNullOrWhiteSpace(filters.CodigoEstadoPago))
                    filter = filter.And(x => x.Pago.Any(p => p.Activo && p.IdEstadoPagoNavigation.Codigo == filters.CodigoEstadoPago));

                if (filters.IdProveedor.HasValue)
                    filter = filter.And(x => x.IdCanchaNavigation.IdProveedor == filters.IdProveedor.Value);
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
                r => r.IdClienteNavigation
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

            var detallesPorReserva = todosLosDetalles
                .GroupBy(d => d.IdReserva)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pagos = await _pagoRepository.FindByAsNoTrackingAsync(
                p => idsReservas.Contains(p.IdReserva ?? 0) && p.Activo,
                p => p.IdEstadoPagoNavigation!
            );

            var pagosPorReserva = pagos
                .GroupBy(d => d.IdReserva ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            var reservasDtos = new List<ReservaClienteDto>();

            foreach (var r in reservasPaginadas.Items)
            {
                var horarios = new List<HorarioDisponibleDto>();

                if (detallesPorReserva.TryGetValue(r.IdReserva, out var detallesReserva))
                {
                    horarios = HorarioHelper.AgruparHorariosDesdeDetalles(detallesReserva);
                }

                Entity.Pago? pagoActivo = null;
                if (pagosPorReserva.TryGetValue(r.IdReserva, out var pagosReserva))
                {
                    pagoActivo = pagosReserva.FirstOrDefault();
                }

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
                    // Cliente
                    NombreCliente = r.IdClienteNavigation?.FirstName + r.IdClienteNavigation?.FirstName,
                    TelefonoCliente = r.IdClienteNavigation?.PhoneNumber,
                    // Horarios
                    Horarios = horarios,
                    // Pago
                    CodigoEstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Codigo ?? "02",
                    EstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Nombre ?? "Pendiente",
                    MontoAdelanto = pagoActivo?.MontoAdelanto ?? 0,
                    MontoPendiente = pagoActivo?.MontoPendiente ?? 0,
                    NumeroRecibo = pagoActivo?.NumeroReferencia,
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
