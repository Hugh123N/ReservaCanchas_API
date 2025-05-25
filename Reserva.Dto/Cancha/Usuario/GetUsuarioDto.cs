namespace Reserva.Dto.Cancha.Usuario
{
    public class GetUsuarioDto : UsuarioDto
    {
        public int IdUsuario { get; set; }
        public bool Activo { get; set; }
    }
}
