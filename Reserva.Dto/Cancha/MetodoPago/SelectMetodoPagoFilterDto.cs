namespace Reserva.Dto.Cancha.MetodoPago
{
    public class SelectMetodoPagoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdMetodoPago { get; set; }
        public bool? Activo { get; set; }
    }
}
