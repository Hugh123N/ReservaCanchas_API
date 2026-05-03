using System.Text.Json.Serialization;
using Reserva.Common;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear un cargo (charge) en Culqi
    /// </summary>
    public class CulqiCreateChargeRequest
    {
        /// <summary>
        /// Monto en centavos (ej: 10000 = S/ 100.00)
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Código de moneda (PEN para soles)
        /// </summary>
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = Constants.CURRENCY.PEN;

        /// <summary>
        /// Descripción del cargo
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Email del cliente
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// ID del token o tarjeta generado en el frontend
        /// </summary>
        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = null!;

        /// <summary>
        /// Metadata adicional del pago (opcional)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Si debe capturarse inmediatamente (por defecto true)
        /// </summary>
        [JsonPropertyName("capture")]
        public bool Capture { get; set; } = true;
    }
}
