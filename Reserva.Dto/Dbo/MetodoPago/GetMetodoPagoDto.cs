namespace Reserva.Dto.Dbo.MetodoPago
{
    public class GetMetodoPagoDto : MetodoPagoDto
    {
        public int IdMetodoPago { get; set; }
        public bool Activo { get; set; }
    }
}
