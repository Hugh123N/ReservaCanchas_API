using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al consultar un charge.
    /// </summary>
    public class CulqiGetChargeResponse
    {
        /// <summary>
        /// Tipo de objeto retornado por Culqi. Ejemplo: "charge".
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Identificador único del charge.
        /// Ejemplo: "chr_test_30JcZR636jeEH5vB".
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Fecha de creación del charge en formato Unix Timestamp (milisegundos).
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Monto original del charge expresado en céntimos.
        /// Ejemplo: 3990 representa S/ 39.90.
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Monto que ha sido reembolsado del charge, expresado en céntimos.
        /// </summary>
        [JsonPropertyName("amount_refunded")]
        public int AmountRefunded { get; set; }

        /// <summary>
        /// Monto actual del charge después de considerar reembolsos,
        /// expresado en céntimos.
        /// </summary>
        [JsonPropertyName("current_amount")]
        public int CurrentAmount { get; set; }

        /// <summary>
        /// Código de moneda utilizado en la transacción.
        /// Ejemplo: "PEN".
        /// </summary>
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = null!;

        /// <summary>
        /// Correo electrónico asociado al charge.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Descripción de la transacción.
        /// Ejemplo: "Subs Mensual".
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Fuente utilizada para realizar el pago.
        /// </summary>
        [JsonPropertyName("source")]
        public CulqiChargeSource Source { get; set; } = null!;
    }

    /// <summary>
    /// Información de la fuente asociada al charge.
    /// </summary>
    public class CulqiChargeSource
    {
        /// <summary>
        /// Tipo de objeto de la fuente.
        /// Ejemplo: "card".
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Identificador de la tarjeta o fuente utilizada.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Indica si la fuente se encuentra activa.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Fecha de creación de la fuente en formato Unix Timestamp (milisegundos).
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Información interna del token asociado a la tarjeta.
        /// </summary>
        [JsonPropertyName("source")]
        public CulqiCardSource Source { get; set; } = null!;
    }

    /// <summary>
    /// Información de la tarjeta asociada a la fuente del charge.
    /// </summary>
    public class CulqiCardSource
    {
        /// <summary>
        /// Tipo de objeto retornado por Culqi.
        /// Ejemplo: "token".
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Identificador del token de la tarjeta.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Indica si el token se encuentra activo.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Tipo de fuente.
        /// Ejemplo: "card".
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Correo electrónico asociado al token.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Número de tarjeta enmascarado.
        /// Ejemplo: "411111******0013".
        /// </summary>
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } = null!;

        /// <summary>
        /// Últimos cuatro dígitos de la tarjeta.
        /// Ejemplo: "0013".
        /// </summary>
        [JsonPropertyName("last_four")]
        public string LastFour { get; set; } = null!;
    }
}
