using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Comision
{
    public class SearchComisionFilterDto
    {
        public decimal? Porcentaje { get; set; }
        public DateTime? FechaInicio { get; set; }
    }
}
