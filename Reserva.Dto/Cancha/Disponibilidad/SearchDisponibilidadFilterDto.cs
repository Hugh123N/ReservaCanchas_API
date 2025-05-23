namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class SearchDisponibilidadFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDisponibilidad { get; set; }
        public bool? Activo { get; set; }
    }
}
