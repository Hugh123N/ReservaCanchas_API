
namespace Reserva.Dto.Cancha.Usuario
{
    public class SearchUsuarioDto: UsuarioDto
    {
        public Guid Id { get; set; }
        public bool Activo { get; set; }
    }
}
