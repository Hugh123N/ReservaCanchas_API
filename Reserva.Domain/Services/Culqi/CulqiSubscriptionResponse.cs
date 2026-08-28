using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al crear una suscripción
    /// </summary>
    public class CulqiSubscriptionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        [JsonPropertyName("next_billing_date")]
        public long? NextBillingDate { get; set; }

        [JsonPropertyName("current_period")]
        public int? CurrentPeriod { get; set; }

        [JsonPropertyName("active_card")]
        public string? ActiveCard { get; set; }

        [JsonPropertyName("plan")]
        public CulqiPlanResponse? Plan { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
