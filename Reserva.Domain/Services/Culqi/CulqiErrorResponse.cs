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
        public string? Object { get; set; } = "error";

        /// <summary>
        /// Tipo de error
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; } = null!;

        /// <summary>
        /// Código específico del error
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Código de denegación proporcionado por el banco.
        /// Ejemplo: insufficient_funds
        /// </summary>
        [JsonPropertyName("decline_code")]
        public string? DeclineCode { get; set; }

        /// <summary>
        /// ID del cargo asociado al error, cuando aplica.
        /// </summary>
        [JsonPropertyName("charge_id")]
        public string? ChargeId { get; set; }

        /// <summary>
        /// Parámetro que provocó el error.
        /// Ejemplo: amount
        /// </summary>
        [JsonPropertyName("param")]
        public string? Param { get; set; }

        /// <summary>
        /// Mensaje del error para el comercio
        /// </summary>
        [JsonPropertyName("merchant_message")]
        public string? MerchantMessage { get; set; } = null!;

        /// <summary>
        /// Mensaje del error para el usuario final
        /// </summary>
        [JsonPropertyName("user_message")]
        public string? UserMessage { get; set; }

    }
}
