using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Comision
{
    public class ComisionDto
    {
        public decimal Porcentaje { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
