using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Cancha.Usuario
{
    public class UpgradeToProveedorDto
    {
        public string? RazonSocial { get; set; }
        public string? Ruc { get; set; }
        public int IdTipoProveedor { get; set; }
    }
}
