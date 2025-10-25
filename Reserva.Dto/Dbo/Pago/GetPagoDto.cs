namespace Reserva.Dto.Dbo.Pago
{
    public class GetPagoDto : PagoDto
    {
        public int IdPago { get; set; }
        public bool Activo { get; set; }
    }
}
