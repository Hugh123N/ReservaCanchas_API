using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Usuario
{
    public class CreateAndLoginDto
    {
        public string ApplicationCode { get; set; } = null!;
        public string? Email { get; set; } 
        public string? Password { get; set; } 
        public string? IdToken { get; set; } 
        public string TypeValidation { get; set; } = null!;
        public string? TipoUser { get; set; }
    }
}
