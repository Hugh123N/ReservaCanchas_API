using AutoMapper;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Dashboard
{
    public class GetDashboardOperadorQueryHandler : QueryHandlerBase<GetDashboardOperadorQuery, GetDashboardOperadorDto>
    {
        private readonly IRepository<Entity.OperadorCancha> _operadorCanchaRepository;
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IUserIdentity _userIdentity;

        public GetDashboardOperadorQueryHandler(
            IMapper mapper,
            IRepository<Entity.OperadorCancha> operadorCanchaRepository,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IUserIdentity userIdentity
        ) : base(mapper)
        {
            _operadorCanchaRepository = operadorCanchaRepository;
            _canchaRepository = canchaRepository;
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<GetDashboardOperadorDto>> HandleQuery(
            GetDashboardOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDashboardOperadorDto>();

            var idOperador = _userIdentity.GetCurrentUserIdNegocio();
            if (idOperador == null || idOperador == 0)
            {
                response.AddErrorResult("No se encontró el operador asociado al usuario.");
                return response;
            }

            // ─── Canchas asignadas al operador ────────────────────────────────────────
            var operadorCanchas = (await _operadorCanchaRepository.FindByAsNoTrackingAsync(
                oc => oc.Activo && oc.IdOperador == idOperador)).ToList();

            var idCanchasList = operadorCanchas.Select(oc => oc.IdCancha).ToList();

            if (!idCanchasList.Any())
            {
                response.UpdateData(BuildEmptyDashboard());
                return response;
            }

            var canchas = (await _canchaRepository.FindByAsNoTrackingAsync(
                c => c.Activo && idCanchasList.Contains(c.IdCancha))).ToList();

            var canchasNombreDict = canchas.ToDictionary(c => c.IdCancha, c => c.Nombre);

            // ─── Rangos de fechas ─────────────────────────────────────────────────────
            var now       = DateTimeOffset.UtcNow;
            var today     = now.UtcDateTime.Date;
            var inicioHoy = new DateTimeOffset(today, TimeSpan.Zero);
            var finHoy    = inicioHoy.AddDays(1);

            // ─── Reservas de hoy (confirmadas) ────────────────────────────────────────
            var reservasHoyConfirmadas = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado &&
                     r.FechaReserva >= inicioHoy &&
                     r.FechaReserva < finHoy,
                r => r.IdClienteNavigation,
                r => r.IdEstadoReservaNavigation)).ToList();

            // ─── Reservas de hoy (todas) para el stat de total ────────────────────────
            var reservasHoyTotal = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.FechaReserva >= inicioHoy &&
                     r.FechaReserva < finHoy)).ToList();

            // ─── Reservas pendientes (sin filtro de fecha) ────────────────────────────
            var reservasPendientes = (await _reservaRepository.FindByAsNoTrackingAsync(
                r => r.Activo &&
                     idCanchasList.Contains(r.IdCancha) &&
                     r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente,
                r => r.IdClienteNavigation,
                r => r.IdEstadoReservaNavigation))
            .OrderBy(r => r.FechaReserva)
            .Take(10)
            .ToList();

            // ─── Detalles de reservas de hoy (para HoraInicio / HoraFin) ─────────────
            var idsReservasHoy = reservasHoyConfirmadas.Select(r => r.IdReserva).ToList();
            var detallesHoy = idsReservasHoy.Any()
                ? (await _detalleReservaRepository.FindByAsNoTrackingAsync(
                    d => d.Activo && idsReservasHoy.Contains(d.IdReserva),
                    d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                    d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!)).ToList()
                : new List<Entity.DetalleReserva>();

            var detalleDict = detallesHoy
                .GroupBy(d => d.IdReserva)
                .ToDictionary(g => g.Key, g => g.First());

            // ─── Canchas con reservasHoy ──────────────────────────────────────────────
            var canchasAsignadasDto = canchas.Select(c => new DashboardCanchaAsignadaOperadorDto
            {
                IdCancha    = c.IdCancha,
                Nombre      = c.Nombre,
                EsPrincipal = false,
                ReservasHoy = reservasHoyTotal.Count(r => r.IdCancha == c.IdCancha)
            }).ToList();

            // ─── Reservas pendientes dto ──────────────────────────────────────────────
            var reservasPendientesDto = reservasPendientes.Select(r =>
            {
                int minutos = Math.Max(0, (int)(r.FechaReserva - now).TotalMinutes);
                return new DashboardReservaPendienteOperadorDto
                {
                    IdReserva         = r.IdReserva,
                    NombreCliente     = $"{r.IdClienteNavigation.FirstName} {r.IdClienteNavigation.LastName}",
                    TelefonoCliente   = r.IdClienteNavigation.PhoneNumber,
                    NombreCancha      = canchasNombreDict.TryGetValue(r.IdCancha, out var nombre) ? nombre : string.Empty,
                    FechaHora         = r.FechaReserva.UtcDateTime.ToString("yyyy-MM-dd HH:mm"),
                    Monto             = r.MontoTotal,
                    MinutosParaInicio = minutos,
                    Prioridad         = minutos < 180 ? "Alta" : minutos < 300 ? "Media" : "Baja"
                };
            }).ToList();

            // ─── Reservas de hoy dto ──────────────────────────────────────────────────
            var reservasHoyDto = reservasHoyConfirmadas
                .OrderBy(r => r.FechaReserva)
                .Select(r =>
                {
                    detalleDict.TryGetValue(r.IdReserva, out var detalle);
                    var horaInicio = detalle?.IdHorarioCanchaNavigation?.IdHoraInicioNavigation?.HoraTexto
                                     ?? r.FechaReserva.UtcDateTime.ToString("HH:mm");
                    var horaFin    = detalle?.IdHorarioCanchaNavigation?.IdHoraFinNavigation?.HoraTexto
                                     ?? string.Empty;

                    return new DashboardReservaHoyOperadorDto
                    {
                        IdReserva     = r.IdReserva,
                        NombreCliente = $"{r.IdClienteNavigation.FirstName} {r.IdClienteNavigation.LastName}",
                        NombreCancha  = canchasNombreDict.TryGetValue(r.IdCancha, out var nombre) ? nombre : string.Empty,
                        HoraInicio    = horaInicio,
                        HoraFin       = horaFin,
                        Estado        = "Confirmado"
                    };
                }).ToList();

            // ─── Ensamblar respuesta ──────────────────────────────────────────────────
            var dashboardDto = new GetDashboardOperadorDto
            {
                Stats = new DashboardStatsOperadorDto
                {
                    ReservasHoy                = reservasHoyTotal.Count,
                    ReservasHoyConfirmadas     = reservasHoyConfirmadas.Count,
                    ReservasPendientesContacto = reservasPendientes.Count,
                    TotalCanchasAsignadas      = canchas.Count
                },
                CanchasAsignadas   = canchasAsignadasDto,
                ReservasPendientes = reservasPendientesDto,
                ReservasHoy        = reservasHoyDto,
                FechaGeneracion    = now.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            response.UpdateData(dashboardDto);
            return response;
        }

        private static GetDashboardOperadorDto BuildEmptyDashboard() => new()
        {
            Stats              = new DashboardStatsOperadorDto(),
            CanchasAsignadas   = new List<DashboardCanchaAsignadaOperadorDto>(),
            ReservasPendientes = new List<DashboardReservaPendienteOperadorDto>(),
            ReservasHoy        = new List<DashboardReservaHoyOperadorDto>(),
            FechaGeneracion    = DateTimeOffset.UtcNow.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }
}
