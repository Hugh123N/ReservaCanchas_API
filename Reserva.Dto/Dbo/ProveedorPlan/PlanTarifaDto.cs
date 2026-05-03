using System;

namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class PlanTarifaDto
    {
        public int IdPlane { get; set; }
        public string Codigo { get; set; } = null!;
        public decimal Precio { get; set; }
        public string Moneda { get; set; } = null!;
        public int DuracionDias { get; set; }
        public decimal? PorcentajeDescuento { get; set; }
        public string TipoCobro { get; set; } = null!;
        public bool? PermiteAutoRenovacion { get; set; }
    }
}
