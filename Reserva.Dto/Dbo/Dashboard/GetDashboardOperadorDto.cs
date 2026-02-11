namespace Reserva.Dto.Dbo.Dashboard
{
    public class GetDashboardOperadorDto
    {
        public DashboardStatsOperadorDto Stats { get; set; } = new();
        public List<DashboardCanchaAsignadaOperadorDto> CanchasAsignadas { get; set; } = new();
        public List<DashboardReservaPendienteOperadorDto> ReservasPendientes { get; set; } = new();
        public List<DashboardReservaHoyOperadorDto> ReservasHoy { get; set; } = new();
        public string FechaGeneracion { get; set; } = string.Empty;
    }
}
