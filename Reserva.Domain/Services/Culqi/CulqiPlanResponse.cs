using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    public class CulqiPlanResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// Código de moneda
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("interval_unit_time")]
        public int Interval { get; set; }

        [JsonPropertyName("interval_count")]
        public int IntervalCount { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class CulqiCreatePlanResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = null!;
    }
}
