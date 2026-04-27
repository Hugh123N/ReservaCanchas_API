namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class GetProveedorPlanCurrentDto : ProveedorPlanDto
    {
        public int IdProveedorPlan { get; set; }
        public bool Activo { get; set; }

        public string? NombrePlan { get; set; }
        public string? DescripcionPlan { get; set; }
        public decimal? PrecioPlan { get; set; }
        public string? NombreTarifa { get; set; }
        public int? DuracionDias { get; set; }
        public string? TipoCobro { get; set; }
    }
}