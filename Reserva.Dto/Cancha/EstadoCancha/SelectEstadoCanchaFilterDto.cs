namespace Reserva.Dto.Cancha.EstadoCancha
{
    public class SelectEstadoCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
