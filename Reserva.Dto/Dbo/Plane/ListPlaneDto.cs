
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Dto.Dbo.Plane
{
    public class ListPlaneDto: PlaneDto
    {
        public int IdPlane { get; set; }
        public List<PlanCaracteristicaDto>? PlanCaracteristicas { get; set; }
        public List<GetPlanTarifaDto>? PlanTarifa { get; set; }
        public List<PlanLimiteDto>? PlanLimite { get; set; }
    }
}
