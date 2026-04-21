namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO que representa un cliente del sistema
    /// </summary>
    public class ClienteDto
    {
        public Guid IdCliente { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public bool Activo { get; set; }
    }
}
