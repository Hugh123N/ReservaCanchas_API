using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al obtener/crear una tarjeta
    /// </summary>
    public class CulqiCardResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        [JsonPropertyName("last_four_digits")]
        public string? LastFourDigits { get; set; }

        [JsonPropertyName("iin")]
        public CulqiCardIin? Iin { get; set; }

        [JsonPropertyName("card_type")]
        public string? CardType { get; set; }

        [JsonPropertyName("expiration_month")]
        public int ExpirationMonth { get; set; }

        [JsonPropertyName("expiration_year")]
        public int ExpirationYear { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class CulqiCardIin
    {
        [JsonPropertyName("bin")]
        public string? Bin { get; set; }

        [JsonPropertyName("card_brand")]
        public string? CardBrand { get; set; }

        [JsonPropertyName("card_type")]
        public string? CardType { get; set; }

        [JsonPropertyName("card_category")]
        public string? CardCategory { get; set; }
    }
}
