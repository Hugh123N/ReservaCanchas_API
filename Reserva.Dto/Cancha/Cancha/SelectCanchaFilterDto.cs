namespace Reserva.Dto.Cancha.Cancha
{
    public class SelectCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
