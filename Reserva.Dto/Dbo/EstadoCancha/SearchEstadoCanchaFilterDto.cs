namespace Reserva.Dto.Dbo.EstadoCancha
{
    public class SearchEstadoCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
