namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO con información de reserva para visualización del cliente
    /// </summary>
    public class ReservaClienteDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; } = null!;
        public DateTimeOffset Fecha { get; set; }
        public decimal Monto { get; set; }

        // Información del estado
        public string EstadoReserva { get; set; } = null!;
        public string CodigoEstadoReserva { get; set; } = null!;

        // Información de la cancha
        public int IdCancha { get; set; }
        public string NombreCancha { get; set; } = null!;
        public string DireccionCancha { get; set; } = null!;
        public string? TelefonoCancha { get; set; }

        // Información del cliente
        public string? NombreCliente { get; set; }
        public string? TelefonoCliente { get; set; }

        // Horarios reservados
        public List<HorarioReservadoDto> Horarios { get; set; } = new();

        // Información del pago
        public string? CodigoEstadoPago { get; set; }
        public string EstadoPago { get; set; } = null!;
        public decimal MontoAdelanto { get; set; }
        public decimal MontoPendiente { get; set; }
        public string? NumeroRecibo { get; set; }

        // Fechas de control
        public DateTimeOffset? FechaExpiracionPreReserva { get; set; }
        public DateTimeOffset FechaCreacion { get; set; }

        // Indicadores
        public bool EstaConfirmada => CodigoEstadoReserva == "02";
        public bool EstaPendiente => CodigoEstadoReserva == "01";
        public bool EstaCancelada => CodigoEstadoReserva == "03";
        public bool EstaExpirada => CodigoEstadoReserva == "04";
        public bool TienePagoPendiente => MontoPendiente > 0;
    }

    public class HorarioReservadoDto
    {
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public string? HorarioFormateado { get; set; }
    }
}
