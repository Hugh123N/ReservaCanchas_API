namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO para la respuesta de disponibilidad semanal de una cancha
    /// </summary>
    public class DisponibilidadSemanalResponseDto
    {
        public CanchaDisponibilidadDto Cancha { get; set; } = new();
        public List<DiaHorarioDto> Horarios { get; set; } = new();
    }

    /// <summary>
    /// Información básica de la cancha con su rango de horarios
    /// </summary>
    public class CanchaDisponibilidadDto
    {
        public int IdCancha { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
    }

    /// <summary>
    /// Horarios disponibles para un día específico
    /// </summary>
    public class DiaHorarioDto
    {
        public string Fecha { get; set; } = string.Empty;
        public int DiaSemana { get; set; }
        public string NombreDia { get; set; } = string.Empty;
        public List<SlotHorarioDto> Slots { get; set; } = new();
    }

    /// <summary>
    /// Slot de horario individual (1 hora)
    /// </summary>
    public class SlotHorarioDto
    {
        public int IdHorarioCancha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public ReservaSlotDto? Reserva { get; set; }
    }

    /// <summary>
    /// Información de reserva asociada a un slot
    /// </summary>
    public class ReservaSlotDto
    {
        public int IdReserva { get; set; }
        public string CodigoReserva { get; set; } = string.Empty;
        public ClienteSlotDto Cliente { get; set; } = new();
        public string Deporte { get; set; } = string.Empty;
        public decimal CantidadHoras { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaExpiracion { get; set; }

        // Estados de reserva
        public string EstadoReserva { get; set; } = string.Empty;
        public string CodigoEstadoReserva { get; set; } = string.Empty;

        // Estados de pago
        public string EstadoPago { get; set; } = string.Empty;
        public string CodigoEstadoPago { get; set; } = string.Empty;

        // Información de pago
        public decimal MontoAdelanto { get; set; }
        public decimal MontoPendiente { get; set; }
        public string? NumeroRecibo { get; set; }

        // Información adicional
        public string? NombreOperadorConfirmo { get; set; }
        public string? Observaciones { get; set; }

        // Información de la cancha
        public string NombreCancha { get; set; } = string.Empty;
        public string? DireccionCancha { get; set; }
        public string? TelefonoCancha { get; set; }

        // Horarios de la reserva (agrupados en bloques consecutivos)
        public List<HorarioDetalleDto> Horarios { get; set; } = new();
    }

    /// <summary>
    /// Detalle de un bloque horario de la reserva
    /// </summary>
    public class HorarioDetalleDto
    {
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string HorarioFormateado { get; set; } = string.Empty;
    }

    /// <summary>
    /// Información del cliente de la reserva
    /// </summary>
    public class ClienteSlotDto
    {
        public string IdCliente { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
