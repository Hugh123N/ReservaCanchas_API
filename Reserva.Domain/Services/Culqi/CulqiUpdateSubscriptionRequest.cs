using System.Collections.Generic;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para actualizar una suscripción en Culqi (cambio de plan, prorrateo)
    /// </summary>
    public class CulqiUpdateSubscriptionRequest
    {
        /// <summary>
        /// ID del nuevo plan al que se cambia la suscripción
        /// </summary>
        public string? PlanId { get; set; }

        /// <summary>
        /// ID de la nueva tarjeta a usar para la suscripción
        /// </summary>
        public string? CardId { get; set; }

        /// <summary>
        /// Metadatos adicionales (opcional)
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
