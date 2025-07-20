using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Cancha.Usuario
{
    public class CreateUsuarioProveedorDto
    {
        public string? UserName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? RazonSocial { get; set; }
        public string? Ruc { get; set; }
        public int IdTipoProveedor { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
