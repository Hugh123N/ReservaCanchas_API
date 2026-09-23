using Reserva.Dto.Dbo.Usuario;

namespace Reserva.Dto.Dbo.Proveedor
{
    public class CreateProveedorDto : ProveedorDto
    {
        public string UserName { get; set; }= null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
