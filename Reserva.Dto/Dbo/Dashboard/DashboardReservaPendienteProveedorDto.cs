namespace Reserva.Dto.Dbo.Dashboard
{
    public class DashboardReservaPendienteProveedorDto
    {
        public int IdReserva { get; set; }

        /// <summary>Nombre completo del cliente</summary>
        public string NombreCliente { get; set; } = null!;

        /// <summary>Nombre de la cancha reservada</summary>
        public string NombreCancha { get; set; } = null!;

        /// <summary>Fecha y hora de la reserva (yyyy-MM-dd HH:mm)</summary>
        public string FechaHora { get; set; } = null!;

        /// <summary>Monto total de la reserva</summary>
        public decimal Monto { get; set; }

        /// <summary>Minutos que faltan para el inicio de la reserva</summary>
        public int MinutosParaInicio { get; set; }
    }
}
