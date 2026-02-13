namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardReservaPendienteProveedorDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public string? TelefonoCliente { get; set; }
        public string NombreCancha { get; set; } = null!;
        public DateTimeOffset FechaReserva { get; set; }
        public decimal Monto { get; set; }
        public int MinutosParaInicio { get; set; }
    }
}
