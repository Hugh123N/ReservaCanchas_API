using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Comision
{
    public class GetComisionDto : ComisionDto
    {
        public int? IdComision { get; set; }

        public bool? Activo { get; set; }
    }
}
