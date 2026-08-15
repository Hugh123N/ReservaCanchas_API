using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al listar tarjetas de un cliente
    /// </summary>
    public class CulqiCardsListResponse
    {
        [JsonPropertyName("data")]
        public List<CulqiCardResponse> Data { get; set; } = new();

        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
