namespace Reserva.Dto.Cancha.Pago
{
    public class GetPagoDto : PagoDto
    {
        public int IdPago { get; set; }
        public bool Activo { get; set; }
    }
}
