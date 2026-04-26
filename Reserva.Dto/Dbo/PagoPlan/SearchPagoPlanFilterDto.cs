namespace Reserva.Dto.Dbo.PagoPlan
{
    public class SearchPagoPlanFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdPagoPlan { get; set; }
        public bool? Activo { get; set; }
    }
}
