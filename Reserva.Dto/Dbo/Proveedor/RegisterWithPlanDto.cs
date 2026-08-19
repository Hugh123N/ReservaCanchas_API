namespace Reserva.Dto.Dbo.Proveedor
{
    /// <summary>
    /// DTO para registro de proveedor con plan gratuito.
    /// Combina datos del usuario/proveedor y del plan a asignar.
    /// </summary>
    public class RegisterWithPlanDto
    {
        // Datos del Proveedor/Usuario
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string? Telefono { get; set; }

        // Datos del Plan
        public int IdPlane { get; set; }
        public int IdPlanTarifa { get; set; }
    }
}
