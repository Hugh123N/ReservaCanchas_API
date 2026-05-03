using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al crear una suscripción
    /// </summary>
    public class CulqiSubscriptionResponse
    {
        /// <summary>
        /// ID único de la suscripción en Culqi
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (subscription)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// ID del plan asociado
        /// </summary>
        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; } = null!;

        /// <summary>
        /// ID del cliente asociado
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        /// <summary>
        /// Estado de la suscripción (active, cancelled, etc.)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Fecha de inicio (timestamp en segundos)
        /// </summary>
        [JsonPropertyName("start_date")]
        public long StartDate { get; set; }

        /// <summary>
        /// Fecha del próximo cobro (timestamp en segundos)
        /// </summary>
        [JsonPropertyName("next_billing_date")]
        public long? NextBillingDate { get; set; }

        /// <summary>
        /// Fecha de finalización (timestamp en segundos, null si es indefinido)
        /// </summary>
        [JsonPropertyName("end_date")]
        public long? EndDate { get; set; }

        /// <summary>
        /// Número de ciclos facturados hasta ahora
        /// </summary>
        [JsonPropertyName("bill_times")]
        public int BillTimes { get; set; }

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Fecha de creación (timestamp en segundos)
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Código de referencia del último cargo
        /// </summary>
        [JsonPropertyName("reference_code")]
        public string? ReferenceCode { get; set; }
    }
}
