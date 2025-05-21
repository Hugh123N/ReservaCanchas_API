using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Comision
{
    public class ComisionFilterDto
    {
        public decimal? Porcentaje { get; set; }
        public DateTime? FechaInicio { get; set; }
    }
}
