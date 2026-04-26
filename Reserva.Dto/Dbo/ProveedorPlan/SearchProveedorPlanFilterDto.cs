namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class SearchProveedorPlanFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdProveedorPlan { get; set; }
        public bool? Activo { get; set; }
    }
}
