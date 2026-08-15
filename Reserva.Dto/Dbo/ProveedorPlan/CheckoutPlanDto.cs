namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class CheckoutPlanDto
    {
        public int IdProveedor { get; set; }
        public int IdPlane { get; set; }
        public int IdPlanTarifa { get; set; }
        public string? CulqiToken { get; set; }
        /// <summary>
        /// Tipo de pago: "card" = tarjeta (suscripción), "order" = yape/otro pago único
        /// </summary>
        public string PaymentType { get; set; } = "card";
        public string Email { get; set; } = null!;
    }
}