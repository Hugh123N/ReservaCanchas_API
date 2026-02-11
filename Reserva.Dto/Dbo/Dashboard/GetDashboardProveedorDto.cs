namespace Reserva.Dto.Dbo.Dashboard
{
    public class GetDashboardProveedorDto
    {
        public DashboardStatsProveedorDto Stats { get; set; } = null!;

        public List<DashboardIngresoMensualDto> IngresosMensuales { get; set; } = new();

        public List<DashboardCanchaPopularDto> CanchasPopulares { get; set; } = new();

        public List<DashboardReservaPendienteProveedorDto> ReservasPendientes { get; set; } = new();

        public string FechaGeneracion { get; set; } = null!;
    }
}
