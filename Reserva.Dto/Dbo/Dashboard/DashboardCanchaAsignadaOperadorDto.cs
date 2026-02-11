namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardCanchaAsignadaOperadorDto
    {
        public int IdCancha { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
        public int ReservasHoy { get; set; }
    }
}
