using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear una tarjeta en Culqi
    /// </summary>
    public class CulqiCreateCardRequest
    {
        [JsonPropertyName("token_id")]
        public string TokenId { get; set; } = null!;

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        [JsonPropertyName("validate")]
        public bool? Validate { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
