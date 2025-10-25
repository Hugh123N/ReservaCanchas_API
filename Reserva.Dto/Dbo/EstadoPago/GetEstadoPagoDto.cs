namespace Reserva.Dto.Dbo.EstadoPago
{
    public class GetEstadoPagoDto : EstadoPagoDto
    {
        public int IdEstadoPago { get; set; }
        public bool Activo { get; set; }
    }
}
