using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Cancha
{
    public class CanchaDto
    {
        public string Nombre { get; set; } = null!;

        public int IdTipo { get; set; }

        public string? Descripcion { get; set; }

        public string? Ubicacion { get; set; }

        public string? Imagen { get; set; }

        public decimal? PrecioHora { get; set; }

        public int? CreadoPor { get; set; }

        public int? IdProveedor { get; set; }

        public int? IdZona { get; set; }
    }
}
