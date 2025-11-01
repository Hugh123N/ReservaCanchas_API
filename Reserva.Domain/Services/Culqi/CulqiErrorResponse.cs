using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de error de Culqi
    /// </summary>
    public class CulqiErrorResponse
    {
        /// <summary>
        /// Tipo de objeto (error)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = "error";

        /// <summary>
        /// Tipo de error
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Mensaje del error para el comercio
        /// </summary>
        [JsonPropertyName("merchant_message")]
        public string MerchantMessage { get; set; } = null!;

        /// <summary>
        /// Mensaje del error para el usuario final
        /// </summary>
        [JsonPropertyName("user_message")]
        public string? UserMessage { get; set; }

        /// <summary>
        /// Código del error
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
