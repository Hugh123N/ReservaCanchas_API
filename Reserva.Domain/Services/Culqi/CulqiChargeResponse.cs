using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al crear un cargo
    /// </summary>
    public class CulqiChargeResponse
    {
        /// <summary>
        /// ID único del cargo en Culqi
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (charge)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Monto en centavos
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Código de moneda
        /// </summary>
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = null!;

        /// <summary>
        /// Email del cliente
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Descripción del cargo
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        /// <summary>
        /// ID de la fuente de pago (token o tarjeta)
        /// </summary>
        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = null!;

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Número de referencia del cargo
        /// </summary>
        [JsonPropertyName("reference_code")]
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// Fecha de creación en timestamp (segundos desde epoch)
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Estado del cargo (captured, pending, etc.)
        /// </summary>
        [JsonPropertyName("outcome")]
        public CulqiOutcome? Outcome { get; set; }
    }

    public class CulqiOutcome
    {
        /// <summary>
        /// Tipo de resultado (venta_exitosa, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Código del comercio
        /// </summary>
        [JsonPropertyName("merchant_code")]
        public string? MerchantCode { get; set; }

        /// <summary>
        /// Mensaje del resultado
        /// </summary>
        [JsonPropertyName("merchant_message")]
        public string? MerchantMessage { get; set; }

        /// <summary>
        /// Mensaje para el usuario
        /// </summary>
        [JsonPropertyName("user_message")]
        public string? UserMessage { get; set; }
    }
}
