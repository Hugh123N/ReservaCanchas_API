namespace Reserva.Dto.Cancha.Disponibilidad
{
    public class SelectDisponibilidadFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDisponibilidad { get; set; }
        public bool? Activo { get; set; }
    }
}
