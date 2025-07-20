using System;
using System.Collections.Generic;

namespace Reserva.Dto.Cancha.Pago
{
    public class PagoDto
    {
        public Guid IdUsuario { get; set; }
        public decimal Monto { get; set; }
        public int? IdMetodoPago { get; set; }
        public int IdEstadoPago { get; set; }
    }
}
