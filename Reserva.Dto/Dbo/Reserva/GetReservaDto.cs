namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO para obtener información completa de una reserva
    /// </summary>
    public class GetReservaDto : ReservaDto
    {
        public int IdReserva { get; set; }
        public bool Activo { get; set; }

        // Lista de horarios reservados
        public List<HorarioReservadoDto> Horarios { get; set; } = new();

        // Información de la cancha
        public string NombreCancha { get; set; } = null!;
        public string DireccionCancha { get; set; } = null!;
        public string? TelefonoCancha { get; set; }

        // Información del cliente
        public string NombreCliente { get; set; } = null!;
        public string? NumeroCliente { get; set; }

        // Estado Reserva con código y nombre
        public string EstadoReserva { get; set; } = null!;
        public string CodigoEstadoReserva { get; set; } = null!;

        // Estado Pago con código y nombre
        public string EstadoPago { get; set; } = null!;
        public string CodigoEstadoPago { get; set; } = null!;

        // Operador que confirmó (si existe)
        public string? NombreOperador { get; set; }

        // Tipo de Deporte
        public string? NombreDeporte { get; set; }

        // Fechas de auditoría
        public DateTimeOffset FechaCreacion { get; set; }
        public DateTimeOffset? FechaModificacion { get; set; }
    }
}
