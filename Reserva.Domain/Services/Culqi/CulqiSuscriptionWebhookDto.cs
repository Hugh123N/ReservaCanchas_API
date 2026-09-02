using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.Culqi
{
    public class CulqiSuscriptionWebhookDto
    {
        [JsonPropertyName("message")]
        public CulqiSuscriptionWebhookMessage Message { get; set; } = null!;
    }

    public class CulqiSuscriptionWebhookMessage
    {
        [JsonPropertyName("object")]
        public CulqiSubscriptionWebhookObject Object { get; set; } = null!;
    }

    public class CulqiSubscriptionWebhookObject
    {
        //SUSCRIPTION CREATE
        [JsonPropertyName("planId")]
        public string? PlanId { get; set; }

        [JsonPropertyName("subsId")]
        public string? SubsId { get; set; }

        [JsonPropertyName("merchantId")]
        public string? MerchantId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        //ADICIONALES PARA CHARGE
        [JsonPropertyName("charId")]
        public string? CharId { get; set; }
        [JsonPropertyName("amount")]
        public int? Amount { get; set; }
        [JsonPropertyName("referenceCode")]
        public string? ReferenceCode { get; set; }
        [JsonPropertyName("next_billing_date")]
        public long? NextBillingDate { get; set; }
    }

}
