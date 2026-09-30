using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Usuario
{
    public enum CheckEmailStatus
    {
        NoExiste = 0,
        ExisteSinProveedor = 1,
        ExisteConProveedor = 2
    }

    public class CheckEmailResultDto
    {
        public CheckEmailStatus Status { get; set; }
        public string? Email { get; set; }
        public Guid? IdUser { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}