namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardReservaPendienteProveedorDto
    {
        public int IdReserva { get; set; }

        public string NombreCliente { get; set; } = null!;

        public string NombreCancha { get; set; } = null!;

        public DateTimeOffset FechaReserva { get; set; }

        public decimal Monto { get; set; }

        /// <summary>Minutos que faltan para el inicio de la reserva</summary>
        public int MinutosParaInicio { get; set; }
    }
}
