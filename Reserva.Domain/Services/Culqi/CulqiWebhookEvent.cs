using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Evento recibido en el webhook de Culqi
    /// </summary>
    public class CulqiWebhookEvent
    {
        /// <summary>
        /// ID del evento
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (event)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Tipo de evento (charge.succeeded, order.status.changed, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Fecha de creación del evento (timestamp en milisegundos)
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Datos del evento (puede ser charge, order, etc.)
        /// </summary>
        [JsonPropertyName("data")]
        public CulqiWebhookData Data { get; set; } = null!;
    }

    public class CulqiWebhookData
    {
        /// <summary>
        /// ID del objeto (charge_id, order_id, etc.)
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (charge, order, etc.)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Monto en centavos
        /// </summary>
        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        [JsonPropertyName("currency_code")]
        public string? CurrencyCode { get; set; }

        /// <summary>
        /// Email del cliente
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Descripción
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Metadata del pago
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Número de referencia
        /// </summary>
        [JsonPropertyName("reference_code")]
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// Estado de la orden (para eventos order.status.changed)
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }

        /// <summary>
        /// Resultado del pago
        /// </summary>
        [JsonPropertyName("outcome")]
        public CulqiOutcome? Outcome { get; set; }

        /// <summary>
        /// Próxima fecha de facturación (timestamp en segundos) - para eventos de suscripción
        /// </summary>
        [JsonPropertyName("next_billing_date")]
        public long? NextBillingDate { get; set; }

        /// <summary>
        /// Fuente del pago (token info) - para identificar tipo de pago (card/yape)
        /// </summary>
        [JsonPropertyName("source")]
        public CulqiSource? Source { get; set; }
    }

    /// <summary>
    /// Información de la fuente del pago (token)
    /// </summary>
    public class CulqiSource
    {
        /// <summary>
        /// Tipo de token: "card" o "yape"
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// ID del token
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
