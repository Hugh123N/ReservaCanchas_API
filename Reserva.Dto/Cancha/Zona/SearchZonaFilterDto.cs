namespace Reserva.Dto.Cancha.Zona
{
    public class SearchZonaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdZona { get; set; }
        public bool? Activo { get; set; }
    }
}
