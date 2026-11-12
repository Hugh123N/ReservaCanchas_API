using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Dto.Dbo.Pago
{
    public class GetPagoDto : PagoDto
    {
        public int IdPago { get; set; }
        public int Revision { get; set; }
        public GetEstadoPagoDto? EstadoPago { get; set; }
    }
}
