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
}