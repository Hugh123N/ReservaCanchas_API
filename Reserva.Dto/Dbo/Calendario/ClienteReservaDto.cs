namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO que representa los datos del cliente para crear una reserva
    /// </summary>
    public class ClienteReservaDto
    {
        /// <summary>
        /// ID del cliente (si ya existe). Null si es nuevo cliente
        /// </summary>
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Nombre completo del cliente (si es nuevo)
        /// </summary>
        public string? NombreCompleto { get; set; }

        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        public string Telefono { get; set; } = null!;

        /// <summary>
        /// Email del cliente (opcional)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Indica si es un cliente nuevo (a crear)
        /// </summary>
        public bool EsNuevoCliente { get; set; }
    }
}
