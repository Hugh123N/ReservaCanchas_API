namespace Reserva.Dto.Dbo.ProveedorPlan
{
    public class RetryPaymentDto
    {
        public int IdProveedorPlan { get; set; }

        /// <summary>
        /// Token de la nueva tarjeta generado por CulqiJS (opcional)
        /// Si se proporciona, se actualizará la tarjeta antes de reintentar el pago
        /// </summary>
        public string? CulqiToken { get; set; }

        /// <summary>
        /// Email del cliente (requerido si se envía CulqiToken)
        /// </summary>
        public string? Email { get; set; }
    }
}