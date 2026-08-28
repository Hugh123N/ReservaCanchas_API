using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear una suscripción en Culqi
    /// Las suscripciones permiten cobros recurrentes automáticos
    /// </summary>
    public class CulqiCreateSubscriptionRequest
    {
        [JsonPropertyName("card_id")]
        public string CardId { get; set; } = null!;

        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; } = null!;

        [JsonPropertyName("tyc")]
        public bool TyC { get; set; } = true;

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
