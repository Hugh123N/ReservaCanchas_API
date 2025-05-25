namespace Reserva.Dto.Cancha.DetallePago
{
    public class GetDetallePagoDto : DetallePagoDto
    {
        public int IdDetallePago { get; set; }
        public bool Activo { get; set; }
    }
}
