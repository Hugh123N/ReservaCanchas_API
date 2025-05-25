namespace Reserva.Dto.Cancha.EstadoPago
{
    public class SearchEstadoPagoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoPago { get; set; }
        public bool? Activo { get; set; }
    }
}
