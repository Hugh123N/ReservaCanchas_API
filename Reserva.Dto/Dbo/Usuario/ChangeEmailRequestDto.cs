namespace Reserva.Dto.Dbo.Usuario
{
    public class ChangeEmailRequestDto
    {
        public string NewEmail { get; set; } = null!;
        public string Host { get; set; } = null!;
    }
}
