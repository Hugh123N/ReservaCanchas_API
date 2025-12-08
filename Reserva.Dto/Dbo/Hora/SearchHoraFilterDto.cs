namespace Reserva.Dto.Dbo.Hora
{
    public class SearchHoraFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdHora { get; set; }
        public bool? Activo { get; set; }
    }
}
