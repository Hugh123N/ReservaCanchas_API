using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Información principal del Charge recibido desde Culqi.
    /// </summary>
    public class CulqiChargeWebhookDto
    {
        /// <summary>
        /// ID único del pago generado en Culqi.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Monto cobrado en centavos.
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Moneda del pago.
        /// Ejemplo: PEN
        /// </summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = null!;

        /// <summary>
        /// Correo asociado al pago.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Descripción enviada al momento de crear el pago.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Información del medio utilizado para realizar el pago.
        /// </summary>
        [JsonPropertyName("source")]
        public CulqiChargeSourceDto? Source { get; set; }

        /// <summary>
        /// Resultado de la autorización del pago.
        /// </summary>
        [JsonPropertyName("outcome")]
        public CulqiChargeOutcomeDto? Outcome { get; set; }

        /// <summary>
        /// Datos propios de ReservaFast enviados al crear el pago.
        /// Aquí identificamos proveedor, plan, tarifa y tipo de pago.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Indica si el pago fue capturado.
        /// </summary>
        [JsonPropertyName("capture")]
        public bool Capture { get; set; }

        /// <summary>
        /// Código de referencia de la operación generado por Culqi.
        /// </summary>
        [JsonPropertyName("referenceCode")]
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// Indica si Culqi detectó el pago como duplicado.
        /// </summary>
        [JsonPropertyName("duplicated")]
        public bool Duplicated { get; set; }

        /// <summary>
        /// Código de autorización generado por el procesador.
        /// Puede ser útil para auditoría o soporte.
        /// </summary>
        [JsonPropertyName("authorizationCode")]
        public string? AuthorizationCode { get; set; }
    }

    /// <summary>
    /// Información del medio utilizado para realizar el pago.
    /// </summary>
    public class CulqiChargeSourceDto
    {
        /// <summary>
        /// ID del token o fuente utilizada en el pago.
        /// Ejemplo: ype_test_D5HlRA1z1culXYpq
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo informado por Culqi.
        /// No usar este campo para determinar si fue Yape o Tarjeta.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Últimos 4 dígitos del medio de pago.
        /// </summary>
        [JsonPropertyName("lastFour")]
        public string? LastFour { get; set; }

        /// <summary>
        /// Indica si la fuente de pago está activa.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    /// <summary>
    /// Resultado de la operación de pago.
    /// </summary>
    public class CulqiChargeOutcomeDto
    {
        /// <summary>
        /// Tipo de resultado de la operación.
        /// Ejemplo: venta_exitosa
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Código de respuesta de Culqi.
        /// Ejemplo: AUT0000
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Mensaje de resultado para el comercio.
        /// </summary>
        [JsonPropertyName("merchantMessage")]
        public string? MerchantMessage { get; set; }
    }

}
