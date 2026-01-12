namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO Response al crear una reserva desde el operador
    /// </summary>
    public class ReservaOperadorResponseDto
    {
        /// <summary>
        /// ID de la reserva creada
        /// </summary>
        public int IdReserva { get; set; }

        /// <summary>
        /// Código de la reserva generado
        /// </summary>
        public string CodigoReserva { get; set; } = null!;

        /// <summary>
        /// Fecha de la reserva
        /// </summary>
        public DateTimeOffset FechaReserva { get; set; }

        /// <summary>
        /// Monto total de la reserva
        /// </summary>
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Estado de la reserva
        /// </summary>
        public string EstadoReserva { get; set; } = null!;

        /// <summary>
        /// ID del pago creado
        /// </summary>
        public int IdPago { get; set; }

        /// <summary>
        /// Estado del pago
        /// </summary>
        public string EstadoPago { get; set; } = null!;

        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        public string NombreCliente { get; set; } = null!;

        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        public string? TelefonoCliente { get; set; }

        /// <summary>
        /// Nombre de la cancha
        /// </summary>
        public string NombreCancha { get; set; } = null!;

        /// <summary>
        /// Fecha de expiración de pre-reserva (si aplica)
        /// </summary>
        public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

        /// <summary>
        /// Observaciones de la reserva
        /// </summary>
        public string? Observaciones { get; set; }
    }
}
