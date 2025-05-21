using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Dbo.Cancha
{
    public class SelectComboCanchaDto
    {
        public int? IdDia { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
