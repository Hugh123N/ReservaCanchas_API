using System.Text.Json.Serialization;
using Reserva.Common;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear un plan en Culqi (para suscripciones)
    /// Un plan define el precio y ciclo de facturación recurrente
    /// </summary>
    public class CulqiCreatePlanRequest
    {
        /// <summary>
        /// ID único del plan (generado por ti, ej: "plan_pro_basic_monthly")
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Nombre del plan
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Monto en centavos (ej: 9900 = S/ 99.00)
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Código de moneda (PEN para soles)
        /// </summary>
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = Constants.CURRENCY.PEN;

        /// <summary>
        /// Intervalo de facturación (days, months, years)
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval { get; set; } = "months";

        /// <summary>
        /// Cantidad de intervalos entre cada cobro (ej: 1 = cada 1 mes)
        /// </summary>
        [JsonPropertyName("interval_count")]
        public int IntervalCount { get; set; } = 1;

        /// <summary>
        /// Ciclos de facturación totales (opcional, null = infinito)
        /// </summary>
        [JsonPropertyName("bill_times")]
        public int? BillTimes { get; set; }

        /// <summary>
        /// Descripción del plan
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
