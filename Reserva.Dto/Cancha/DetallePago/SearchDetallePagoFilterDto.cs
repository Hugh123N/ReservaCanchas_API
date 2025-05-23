namespace Reserva.Dto.Cancha.DetallePago
{
    public class SearchDetallePagoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDetallePago { get; set; }
        public bool? Activo { get; set; }
    }
}
