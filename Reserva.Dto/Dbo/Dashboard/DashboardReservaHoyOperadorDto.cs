namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardReservaHoyOperadorDto
    {
        public int IdReserva { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreCancha { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
