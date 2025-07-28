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
        [Required]
        [MaxLength(255)]
        public string? RazonSocial { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Ruc { get; set; }

        [Required]
        public int IdTipoProveedor { get; set; }
    }
}
