namespace Reserva.Dto.Dbo.EstadoUsuario
{
    public class GetEstadoUsuarioDto : EstadoUsuarioDto
    {
        public int IdEstadoUsuario { get; set; }
        public bool Activo { get; set; }
    }
}
