namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO que representa un cliente del sistema
    /// </summary>
    public class ClienteDto
    {
        /// <summary>
        /// ID del cliente (GUID de AspNetUsers)
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        public string NombreCompleto { get; set; } = null!;

        /// <summary>
        /// Nombre del cliente
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Apellido del cliente
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Email del cliente
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Indica si el cliente está activo
        /// </summary>
        public bool Activo { get; set; }
    }
}
