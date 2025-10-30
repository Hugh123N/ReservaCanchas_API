namespace Reserva.Dto.Dbo.Usuario
{
    public class UpdateTelefonoDto
    {
        public Guid IdUsuario { get; set; }
        public string Telefono { get; set; } = null!;
    }
}
