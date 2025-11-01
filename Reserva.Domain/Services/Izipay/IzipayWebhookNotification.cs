namespace Reserva.Domain.Services.Izipay
{
    /// <summary>
    /// Notificación IPN/Webhook que envía Izipay cuando cambia el estado de un pago
    /// </summary>
    public class IzipayWebhookNotification
    {
        /// <summary>
        /// ID de la transacción en Izipay
        /// </summary>
        public string TransactionId { get; set; } = null!;

        /// <summary>
        /// ID de la orden en tu sistema (el que enviaste en CreatePayment)
        /// </summary>
        public string OrderId { get; set; } = null!;

        /// <summary>
        /// Estado del pago: PAID, PENDING, FAILED, CANCELLED
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Monto en céntimos
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Moneda (PEN)
        /// </summary>
        public string Currency { get; set; } = null!;

        /// <summary>
        /// Método de pago utilizado (YAPE, PLIN, CARD, etc.)
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Número de operación del método de pago (ej: código de Yape)
        /// </summary>
        public string? OperationNumber { get; set; }

        /// <summary>
        /// Timestamp de la transacción
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Firma HMAC-SHA256 para validar autenticidad del webhook
        /// (viene en header X-Signature o en el body)
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>
        /// Datos adicionales de la transacción
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
