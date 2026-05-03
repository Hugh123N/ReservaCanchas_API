using Reserva.Dto.Dbo.Plane;

namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class GetProveedorPlanCurrentDto : ProveedorPlanDto
    {
        public int IdProveedorPlan { get; set; }
        public GetPlaneDto Plan { get; set; } = null!;
        public GetPlanTarifaDto PlanTarifas { get; set; } = null!;
        public List<PlanCaracteristicaDto>? PlanCaracteristicas { get; set; }
        public List<PlanLimiteDto>? Limites { get; set; }
    }
}