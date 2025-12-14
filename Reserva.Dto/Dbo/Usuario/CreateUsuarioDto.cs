namespace Reserva.Dto.Dbo.Usuario
{
    public class CreateUsuarioDto
    {
        public string? UserName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IEnumerable<Guid>? RoleIds { get; set; }

        public string? Host { get; set; } // Solo cuando el Proveedor crea Su Operador
    }
}
