using AutoMapper;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Dashboard
{
    public class GetDashboardProveedorQueryHandler : QueryHandlerBase<GetDashboardProveedorQuery, GetDashboardProveedorDto>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.Pago> _pagoRepository;
        private readonly IUserIdentity _userIdentity;

        private static readonly string[] NombresMeses =
            { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic" };

        public GetDashboardProveedorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.Pago> pagoRepository,
            IUserIdentity userIdentity
        ) : base(mapper)
        {
            _canchaRepository = canchaRepository;
            _reservaRepository = reservaRepository;
            _pagoRepository = pagoRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<GetDashboardProveedorDto>> HandleQuery(
            GetDashboardProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDashboardProveedorDto>();

            var idProveedor = _userIdentity.GetCurrentUserIdNegocio();
            if (idProveedor == null || idProveedor == 0)
            {
                response.AddErrorResult("No se encontró el proveedor asociado al usuario.");
                return response;
            }

            // ─── Canchas del proveedor ────────────────────────────────────────────────
            var canchas = (await _canchaRepository.FindByAsNoTrackingAsync(
                c => c.Activo && c.IdProveedor == idProveedor)).ToList();

            var idCanchasList = canchas.Select(c => c.IdCancha).ToList();

            if (!idCanchasList.Any())
            {
                response.UpdateData(BuildEmptyDashboard());
                return response;
            }

            var canchasNombreDict = canchas.ToDictionary(c => c.IdCancha, c => c.Nombre);

            // ─── Rangos de fechas ─────────────────────────────────────────────────────
            var now             = DateTimeOffset.UtcNow;
            var today           = now.UtcDateTime.Date;
            var inicioHoy       = new DateTimeOffset(today, TimeSpan.Zero);
            var finHoy          = inicioHoy.AddDays(1);
            var inicioMesActual = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var finMesActual    = inicioMesActual.AddMonths(1);
            var inicioMesPasado = inicioMesActual.AddMonths(-1);
            var inicio6Meses    = inicioMesActual.AddMonths(-5);

            // ─── Reservas del mes actual ──────────────────────────────────────────────
            var reservasMesActual = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo && idCanchasList.Contains(r.IdCancha) && 
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado &&
                     r.FechaReserva >= inicioMesActual &&
                     r.FechaReserva < finMesActual)).ToList();

            // ─── Todas las reservas pendientes (sin filtro de fecha) ──────────────────
            var todasReservasPendientes = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente)).ToList();

            // ─── Stats básicos ────────────────────────────────────────────────────────
            int reservasMesCount           = reservasMesActual.Count;
            int reservasPendientesAtencion = todasReservasPendientes.Count;
            int reservasHoy                = reservasMesActual.Count(r => r.FechaReserva >= inicioHoy && r.FechaReserva < finHoy);

            // ─── Pagos del mes actual (pagados + parciales) ───────────────────────────
            var idReservasMes = reservasMesActual.Select(r => r.IdReserva).ToList();
            var pagosMesActual = idReservasMes.Any()
                ? (await _pagoRepository.FindByAsNoTrackingAsync(
                    p => p.Activo &&
                         p.IdReserva.HasValue &&
                         idReservasMes.Contains(p.IdReserva!.Value) &&
                         (p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Parcial ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Cancelado))).ToList()
                : new List<Entity.Pago>();

            // Ingresos cobrados = MontoAdelanto − MontoReembolso
            decimal ingresosMesActual = pagosMesActual.Sum(p => p.MontoAdelanto - (p.MontoReembolso ?? 0));

            // ─── Ingresos mes pasado (para calcular variación) ────────────────────────
            var reservasMesPasado = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.FechaReserva >= inicioMesPasado &&
                     r.FechaReserva < inicioMesActual)).ToList();

            var idReservasMesPasado = reservasMesPasado.Select(r => r.IdReserva).ToList();
            var pagosMesPasado = idReservasMesPasado.Any()
                ? (await _pagoRepository.FindByAsNoTrackingAsync(
                    p => p.Activo &&
                         p.IdReserva.HasValue &&
                         idReservasMesPasado.Contains(p.IdReserva!.Value) &&
                         (p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Parcial ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Cancelado))).ToList()
                : new List<Entity.Pago>();

            decimal ingresosMesPasado = pagosMesPasado.Sum(p => p.MontoAdelanto - (p.MontoReembolso ?? 0));
            decimal variacionIngresosMes = ingresosMesPasado > 0
                ? Math.Round((ingresosMesActual - ingresosMesPasado) / ingresosMesPasado * 100, 1)
                : 0;

            // ─── Tasa de ocupación promedio ───────────────────────────────────────────
            // (pares únicos cancha-día con reservas confirmadas) / (totalCanchas × díasEnMes) × 100
            // Se consulta directo a DB para usar la navegación en el filtro EF
            var reservasConfirmadasMes = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.FechaReserva >= inicioMesActual &&
                     r.FechaReserva < finMesActual &&
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado)).ToList();

            int diasConReservas = reservasConfirmadasMes
                .Select(r => (r.IdCancha, r.FechaReserva.UtcDateTime.Date))
                .Distinct().Count();

            int diasEnMes = DateTime.DaysInMonth(today.Year, today.Month);
            double tasaOcupacion = canchas.Count > 0 && diasEnMes > 0
                ? Math.Round((double)diasConReservas / (canchas.Count * diasEnMes) * 100, 1)
                : 0;

            // ─── Ingresos últimos 6 meses (1 query reservas + 1 query pagos) ──────────
            var reservas6Meses = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.FechaReserva >= inicio6Meses &&
                     r.FechaReserva < finMesActual)).ToList();

            var idReservas6Meses = reservas6Meses.Select(r => r.IdReserva).ToList();
            var pagos6Meses = idReservas6Meses.Any()
                ? (await _pagoRepository.FindByAsNoTrackingAsync(
                    p => p.Activo &&
                         p.IdReserva.HasValue &&
                         idReservas6Meses.Contains(p.IdReserva!.Value) &&
                         (p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Parcial ||
                          p.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Cancelado))).ToList()
                : new List<Entity.Pago>();

            var reservaFechaDict = reservas6Meses.ToDictionary(r => r.IdReserva, r => r.FechaReserva);

            var ingresosMensuales = Enumerable.Range(0, 6).Select(i =>
            {
                var mesInicio = inicioMesActual.AddMonths(-(5 - i));
                var mesFin    = mesInicio.AddMonths(1);
                var monto     = pagos6Meses
                    .Where(p => reservaFechaDict.TryGetValue(p.IdReserva!.Value, out var fecha) &&
                                fecha >= mesInicio && fecha < mesFin)
                    .Sum(p => p.MontoAdelanto - (p.MontoReembolso ?? 0));

                return new DashboardIngresoMensualDto
                {
                    Mes   = NombresMeses[mesInicio.Month - 1],
                    Monto = monto
                };
            }).ToList();

            // ─── Canchas más populares ────────────────────────────────────────────────
            var canchasPopulares = canchas.Select(c =>
            {
                var reservasCancha   = reservasMesActual.Where(r => r.IdCancha == c.IdCancha).ToList();
                var idReservasCancha = reservasCancha.Select(r => r.IdReserva).ToHashSet();
                var ingresos = pagosMesActual
                    .Where(p => p.IdReserva.HasValue && idReservasCancha.Contains(p.IdReserva.Value))
                    .Sum(p => p.MontoAdelanto - (p.MontoReembolso ?? 0));

                return (c.IdCancha, c.Nombre, TotalReservas: reservasCancha.Count, Ingresos: ingresos);
            })
            .OrderByDescending(x => x.TotalReservas)
            .ToList();

            int maxReservas = canchasPopulares.Any() ? Math.Max(canchasPopulares.Max(x => x.TotalReservas), 1) : 1;

            var canchasPopularesDto = canchasPopulares.Select((c, idx) => new DashboardCanchaPopularDto
            {
                Posicion          = idx + 1,
                Nombre            = c.Nombre,
                TotalReservas     = c.TotalReservas,
                Porcentaje        = (int)Math.Round((double)c.TotalReservas / maxReservas * 100),
                IngresosGenerados = c.Ingresos
            }).ToList();

            // ─── Reservas pendientes próximas con detalle de cliente ──────────────────
            var reservasPendientesDetalle = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente &&
                     r.FechaReserva >= now,
                x => x.IdClienteNavigation))
            .OrderBy(r => r.FechaReserva)
            .Select(r => new DashboardReservaPendienteProveedorDto
            {
                IdReserva         = r.IdReserva,
                CodigoReserva     = r.CodigoReserva,
                NombreCliente     = $"{r.IdClienteNavigation.FirstName} {r.IdClienteNavigation.LastName}",
                TelefonoCliente   = r.IdClienteNavigation.PhoneNumber,
                NombreCancha      = canchasNombreDict.TryGetValue(r.IdCancha, out var nombre) ? nombre : string.Empty,
                FechaReserva      = r.FechaReserva,
                Monto             = r.MontoTotal,
                MinutosParaInicio = Math.Max(0, (int)(r.FechaReserva - now).TotalMinutes)
            }).ToList();

            // ─── Ensamblar respuesta ──────────────────────────────────────────────────
            var dashboardDto = new GetDashboardProveedorDto
            {
                Stats = new DashboardStatsProveedorDto
                {
                    IngresosMesActual          = ingresosMesActual,
                    VariacionIngresosMes       = variacionIngresosMes,
                    ReservasMes                = reservasMesCount,
                    ReservasPendientesAtencion = reservasPendientesAtencion,
                    ReservasHoy                = reservasHoy,
                    TasaOcupacionPromedio      = tasaOcupacion
                },
                IngresosMensuales  = ingresosMensuales,
                CanchasPopulares   = canchasPopularesDto,
                ReservasPendientes = reservasPendientesDetalle,
                FechaGeneracion    = now.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            response.UpdateData(dashboardDto);
            return response;
        }

        private static GetDashboardProveedorDto BuildEmptyDashboard() => new()
        {
            Stats              = new DashboardStatsProveedorDto(),
            IngresosMensuales  = new List<DashboardIngresoMensualDto>(),
            CanchasPopulares   = new List<DashboardCanchaPopularDto>(),
            ReservasPendientes = new List<DashboardReservaPendienteProveedorDto>(),
            FechaGeneracion    = DateTimeOffset.UtcNow.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }
}
