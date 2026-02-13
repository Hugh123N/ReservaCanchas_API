namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardReservaPendienteOperadorDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public string? TelefonoCliente { get; set; }
        public string NombreCancha { get; set; } = string.Empty;
        public DateTimeOffset FechaReserva { get; set; }
        public decimal Monto { get; set; }
        public int MinutosParaInicio { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }
}
