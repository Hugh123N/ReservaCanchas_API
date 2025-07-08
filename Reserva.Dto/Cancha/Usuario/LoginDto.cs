namespace Reserva.Dto.User
{
    public class LoginDto
    {
        public string ApplicationCode { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
