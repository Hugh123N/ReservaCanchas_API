using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Cancha
{
    public class SearchCanchaFilterDto
    {
        public string? Departamento { get; set; }
        public string? Distrito { get; set; }
        public string? Zona { get; set; }
        public string? Nombre { get; set; }
    }
}
