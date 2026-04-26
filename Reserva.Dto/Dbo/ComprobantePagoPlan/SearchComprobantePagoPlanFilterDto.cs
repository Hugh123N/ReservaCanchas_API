namespace Reserva.Dto.Dbo.ComprobantePagoPlan
{
    public class SearchComprobantePagoPlanFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdComprobantePagoPlan { get; set; }
        public bool? Activo { get; set; }
    }
}
