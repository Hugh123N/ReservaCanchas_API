using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Pago
{
    public class PagoDto
    {
        public int? IdReserva { get; set; }

        public int? IdPlan { get; set; }

        public string Moneda { get; set; } = null!;

        public string? CodigoOperacion { get; set; }
        public decimal Monto { get; set; }
        public string? NumeroReferencia { get; set; }
        public int? IdMetodoPago { get; set; }
        public int IdEstadoPago { get; set; }
    }
}
