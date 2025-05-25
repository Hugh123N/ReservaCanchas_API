using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.GananciaProveedor
{
    public class GananciaProveedorDto
    {
        public int IdReserva { get; set; }
        public int IdProveedor { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Comision { get; set; }
        public decimal GananciaNeta { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
