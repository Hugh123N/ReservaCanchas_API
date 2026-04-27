namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class CheckoutPlanDto
    {
        public int IdProveedor { get; set; }
        public int IdPlane { get; set; }
        public int IdPlanTarifa { get; set; }
        public string? CulqiToken { get; set; }
        public string Email { get; set; } = null!;
    }

    public class CheckoutResponseDto
    {
        public int IdProveedorPlan { get; set; }
        public string? CulqiChargeId { get; set; }
        public string? ReferenceCode { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "PEN";
        public string Estado { get; set; } = "PENDIENTE";
        public DateTimeOffset? FechaExpiracion { get; set; }
    }
}