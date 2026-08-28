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
        /// Nombre del plan
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Monto en centavos (ej: 9900 = S/ 99.00)
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Código de moneda (PEN para soles)
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = Constants.CURRENCY.PEN;

        /// <summary>
        /// Intervalo de facturación (days, months, years)
        /// </summary>
        [JsonPropertyName("interval_unit_time")]
        public int Interval { get; set; } = 3;  //mes por defecto

        /// <summary>
        /// Cantidad de intervalos entre cada cobro (ej: 1 = cada 1 mes)
        /// </summary>
        [JsonPropertyName("interval_count")]
        public int IntervalCount { get; set; } = 1; //mes por defecto

        [JsonPropertyName("initial_cycles")]
        public CulqiInitialCycles? InitialCycles { get; set; }

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class CulqiInitialCycles
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("has_initial_charge")]
        public bool HasInitialCharge { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("interval_unit_time")]
        public int IntervalUnitTime { get; set; } = 3;  //mes por defecto
    }
}
