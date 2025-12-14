namespace Reserva.Dto.Dbo.Usuario
{
    public class GetUsuarioDto : UsuarioDto
    {
        public Guid Id { get; set; }
        public int? IdEstadoUsuario { get; set; }
        public IEnumerable<Guid> RoleIds { get; set; } = Array.Empty<Guid>();
    }
}
