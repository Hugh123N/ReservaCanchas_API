namespace Reserva.Dto.Cancha.EstadoUsuario
{
    public class GetEstadoUsuarioDto : EstadoUsuarioDto
    {
        public int IdEstadoUsuario { get; set; }
        public bool Activo { get; set; }
    }
}
