using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    /// Respuesta de Culqi al crear un cliente
    public class CulqiCustomerResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = null!;

        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        /// Fecha de creación como Unix Timestamp en milisegundos
        [JsonPropertyName("creation_date")]
        public long CreationDate { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
    }
}
