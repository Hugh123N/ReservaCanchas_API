namespace Reserva.Dto.Dbo.DetallePago
{
    public class GetDetallePagoDto : DetallePagoDto
    {
        public int IdDetallePago { get; set; }
        public bool Activo { get; set; }
    }
}
