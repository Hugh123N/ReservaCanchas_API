namespace Reserva.Dto.Cancha.Distrito
{
    public class SearchDistritoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDistrito { get; set; }
        public bool? Activo { get; set; }
    }
}
