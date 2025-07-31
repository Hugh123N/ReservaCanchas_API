using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class GetHorarioDisponible
    {
        public int IdCancha { get; set; }
        public DateTime Fecha { get; set; }
    }
}
