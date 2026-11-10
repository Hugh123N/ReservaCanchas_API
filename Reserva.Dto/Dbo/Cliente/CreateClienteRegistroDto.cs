namespace Reserva.Dto.Dbo.Cliente
{
    public class CreateClienteRegistroDto
    {
        public string? UserName { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string TipoDoc { get; set; } = "DNI";
        public string? NroDoc { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
