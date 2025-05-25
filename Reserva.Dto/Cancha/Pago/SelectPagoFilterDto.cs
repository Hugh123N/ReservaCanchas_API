namespace Reserva.Dto.Cancha.Pago
{
    public class SelectPagoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdPago { get; set; }
        public bool? Activo { get; set; }
    }
}
