namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class ChangePlanDto
    {
        public int IdProveedorPlan { get; set; }
        public int IdProveedor { get; set; }
        public int IdNuevoPlane { get; set; }
        public int IdNuevaPlanTarifa { get; set; }
        public string? CulqiToken { get; set; }
        /// <summary>
        /// Tipo de pago: "card" = tarjeta (suscripción), "order" = yape/otro pago único
        /// </summary>
        public string PaymentType { get; set; } = "card";
        public string? Email { get; set; }
    }

    public class ChangePlanResponseDto
    {
        public int IdProveedorPlan { get; set; }
        public int IdNuevoPlane { get; set; }
        public int IdNuevaPlanTarifa { get; set; }
        public string? CulqiSubscriptionId { get; set; }
        public decimal MontoProrrateado { get; set; }
        public decimal SaldoAFavor { get; set; }
        public string Moneda { get; set; } = "PEN";
        public string Estado { get; set; } = "ACTIVE";
        public DateTimeOffset? NuevaFechaFin { get; set; }
        public DateTimeOffset? NuevaFechaProximoCobro { get; set; }
        public bool EsUpgrade { get; set; }
    }
}
