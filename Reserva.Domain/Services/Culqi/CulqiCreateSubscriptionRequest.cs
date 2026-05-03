using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear una suscripción en Culqi
    /// Las suscripciones permiten cobros recurrentes automáticos
    /// </summary>
    public class CulqiCreateSubscriptionRequest
    {
        /// <summary>
        /// ID del plan en Culqi (previamente creado)
        /// </summary>
        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; } = null!;

        /// <summary>
        /// ID del cliente en Culqi (previamente creado)
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        /// <summary>
        /// ID de la tarjeta o fuente de pago (opcional si el cliente ya tiene una)
        /// </summary>
        [JsonPropertyName("card_id")]
        public string? CardId { get; set; }

        /// <summary>
        /// Metadata adicional de la suscripción
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Fecha de inicio de la suscripción (timestamp en segundos, opcional)
        /// Si no se especifica, comienza inmediatamente
        /// </summary>
        [JsonPropertyName("start_date")]
        public long? StartDate { get; set; }

        /// <summary>
        /// Número de ciclos de facturación (opcional, null = infinito)
        /// </summary>
        [JsonPropertyName("bill_times")]
        public int? BillTimes { get; set; }

        /// <summary>
        /// URL de retorno en caso de fallo de pago (opcional)
        /// </summary>
        [JsonPropertyName("url_failure")]
        public string? UrlFailure { get; set; }

        /// <summary>
        /// URL de retorno en caso de éxito de pago (opcional)
        /// </summary>
        [JsonPropertyName("url_success")]
        public string? UrlSuccess { get; set; }
    }
}
