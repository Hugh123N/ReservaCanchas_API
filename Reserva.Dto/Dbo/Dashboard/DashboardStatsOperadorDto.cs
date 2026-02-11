namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardStatsOperadorDto
    {
        public int ReservasHoy { get; set; }
        public int ReservasHoyConfirmadas { get; set; }
        public int ReservasPendientesContacto { get; set; }
        public int TotalCanchasAsignadas { get; set; }
    }
}
