using System;
using System.Collections.Generic;

namespace Reserva.Dto.Dbo.Pago
{
    public class PagoDto
    {
        public int? IdReserva { get; set; }
        public string Moneda { get; set; } = null!;

        public decimal Monto { get; set; }
        public decimal MontoAdelanto { get; set; }
        public decimal MontoPendiente { get; set; }
        public decimal? MontoReembolso { get; set; }
        public int IdEstadoPago { get; set; }
    }
}
