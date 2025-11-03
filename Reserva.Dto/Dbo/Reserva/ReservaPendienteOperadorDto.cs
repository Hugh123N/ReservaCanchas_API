namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO con información completa de una reserva pendiente para el operador
    /// Incluye datos del cliente para facilitar el contacto
    /// </summary>
    public class ReservaPendienteOperadorDto
    {
        /// <summary>
        /// ID de la reserva
        /// </summary>
        public int IdReserva { get; set; }

        /// <summary>
        /// Código único de la reserva
        /// </summary>
        public string? CodigoReserva { get; set; }

        /// <summary>
        /// Fecha de la reserva
        /// </summary>
        public DateTimeOffset Fecha { get; set; }

        /// <summary>
        /// Hora de inicio (agregado desde ReservaDetalle)
        /// </summary>
        public string? HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin (agregado desde ReservaDetalle)
        /// </summary>
        public string? HoraFin { get; set; }

        /// <summary>
        /// Monto total de la reserva
        /// </summary>
        public decimal? Monto { get; set; }

        /// <summary>
        /// Fecha de creación de la reserva
        /// </summary>
        public DateTimeOffset FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de expiración de la pre-reserva
        /// </summary>
        public DateTimeOffset? FechaExpiracion { get; set; }

        /// <summary>
        /// Horas restantes para expiración
        /// </summary>
        public double HorasRestantes { get; set; }

        /// <summary>
        /// Nombre de la cancha
        /// </summary>
        public string? NombreCancha { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        public string? NombreCliente { get; set; }

        /// <summary>
        /// Email del cliente
        /// </summary>
        public string? EmailCliente { get; set; }

        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        public string? TelefonoCliente { get; set; }

        /// <summary>
        /// Estado de urgencia (crítico si quedan menos de 6 horas)
        /// </summary>
        public string? NivelUrgencia { get; set; }
    }
}
