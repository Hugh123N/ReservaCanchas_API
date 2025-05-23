namespace Reserva.Dto.Cancha.Provincia
{
    public class SearchProvinciaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdProvincia { get; set; }
        public bool? Activo { get; set; }
    }
}
