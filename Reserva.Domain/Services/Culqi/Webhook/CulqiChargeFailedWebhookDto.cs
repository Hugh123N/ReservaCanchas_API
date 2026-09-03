using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Información del error cuando un Charge de Culqi falla.
    /// </summary>
    public class CulqiChargeFailedWebhookDto
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; } = null!;

        [JsonPropertyName("type")]
        public string? Type { get; set; } = null!;

        [JsonPropertyName("chargeId")]
        public string? ChargeId { get; set; } = null!;

        [JsonPropertyName("code")]
        public string? Code { get; set; } = null!;

        [JsonPropertyName("declineCode")]
        public string? DeclineCode { get; set; } = null!;

        [JsonPropertyName("merchantMessage")]
        public string? MerchantMessage { get; set; } = null!;

        //Falta autenticacion

        [JsonPropertyName("userMessage")]
        public string? UserMessage { get; set; } = null!;

        /// <summary>
        /// Código que indica la acción recomendada para el pago.
        /// Ejemplo: REVIEW
        /// </summary>
        [JsonPropertyName("actionCode")]
        public string? ActionCode{ get; set; }
    }
}
