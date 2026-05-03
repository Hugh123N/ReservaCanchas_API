using System;

namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class PlanLimiteDto
    {
        public int IdPlane { get; set; }
        public string Codigo { get; set; } = null!;
        public int Valor { get; set; }
    }
}
