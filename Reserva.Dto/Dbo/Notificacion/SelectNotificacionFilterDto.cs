namespace Reserva.Dto.Dbo.Notificacion
{
    public class SelectNotificacionFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdNotificacion { get; set; }
        public bool? Activo { get; set; }
    }
}
