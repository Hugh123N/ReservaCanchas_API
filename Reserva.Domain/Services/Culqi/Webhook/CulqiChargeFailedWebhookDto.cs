using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Información del error cuando un Charge de Culqi falla.
    /// </summary>
    public class CulqiChargeFailedWebhookDto
    {
        /// <summary>
        /// Mensaje del error que puede mostrarse al usuario.
        /// Ejemplo: El usuario necesita autenticarse
        /// </summary>
        [JsonPropertyName("userMessage")]
        public string? UserMessage { get; set; }

        /// <summary>
        /// Código que indica la acción recomendada para el pago.
        /// Ejemplo: REVIEW
        /// </summary>
        [JsonPropertyName("actionCode")]
        public string? ActionCode { get; set; }
    }

}
