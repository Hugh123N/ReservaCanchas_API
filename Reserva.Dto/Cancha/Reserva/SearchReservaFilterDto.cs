namespace Reserva.Dto.Cancha.Reserva
{
    public class SearchReservaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdReserva { get; set; }
        public bool? Activo { get; set; }
    }
}
