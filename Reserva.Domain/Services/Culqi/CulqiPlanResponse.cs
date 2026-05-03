using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al crear un plan
    /// </summary>
    public class CulqiPlanResponse
    {
        /// <summary>
        /// ID único del plan en Culqi
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (plan)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Nombre del plan
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

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
        /// Intervalo de facturación
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval { get; set; } = null!;

        /// <summary>
        /// Cantidad de intervalos
        /// </summary>
        [JsonPropertyName("interval_count")]
        public int IntervalCount { get; set; }

        /// <summary>
        /// Ciclos de facturación totales
        /// </summary>
        [JsonPropertyName("bill_times")]
        public int? BillTimes { get; set; }

        /// <summary>
        /// Descripción
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Fecha de creación (timestamp en segundos)
        /// </summary>
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Estado del plan (active, etc.)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;
    }
}
