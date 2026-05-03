using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Request para crear un cliente en Culqi
    /// Necesario para usar suscripciones
    /// </summary>
    public class CulqiCreateCustomerRequest
    {
        /// <summary>
        /// Email del cliente
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// ID externo del cliente (tu ID interno, ej: "prov_123")
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Nombres del cliente
        /// </summary>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Apellidos del cliente
        /// </summary>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Número de teléfono
        /// </summary>
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Metadata adicional
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
