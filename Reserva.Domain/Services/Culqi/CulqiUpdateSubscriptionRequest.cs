using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para actualizar una suscripción en Culqi (cambio de plan, prorrateo)
    /// </summary>
    public class CulqiUpdateSubscriptionRequest
    {
        [JsonPropertyName("card_id")]
        public string? CardId { get; set; }

        public Dictionary<string, string>? Metadata { get; set; }
    }
}
