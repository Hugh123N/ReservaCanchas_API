namespace Reserva.Dto.Cancha.Notificacion
{
    public class SearchNotificacionFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdNotificacion { get; set; }
        public bool? Activo { get; set; }
    }
}
