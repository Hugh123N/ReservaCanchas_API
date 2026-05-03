using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Respuesta de Culqi al crear un cliente
    /// </summary>
    public class CulqiCustomerResponse
    {
        /// <summary>
        /// ID único del cliente en Culqi
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Tipo de objeto (customer)
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        /// <summary>
        /// Email del cliente
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Código externo
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Nombres
        /// </summary>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Apellidos
        /// </summary>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

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
    }
}
