namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class CalculateProrationDto
    {
        public int IdProveedorPlan { get; set; }
        public int IdNuevaPlanTarifa { get; set; }
    }

    public class CalculateProrationResponseDto
    {
        public bool EsUpgrade { get; set; }
        public decimal MontoProrrateo { get; set; }
        public decimal CreditoPlanActual { get; set; }
        public decimal CargoPlanNuevo { get; set; }
        public int DiasRestantes { get; set; }
        public decimal SaldoAFavor { get; set; }
        public string Moneda { get; set; } = "PEN";
        public string NombrePlanActual { get; set; } = string.Empty;
        public string NombrePlanNuevo { get; set; } = string.Empty;
        public decimal PrecioPlanActual { get; set; }
        public decimal PrecioPlanNuevo { get; set; }
        public DateTimeOffset? FechaFinActual { get; set; }
        public DateTimeOffset? FechaProximoCobro { get; set; }
    }
}
