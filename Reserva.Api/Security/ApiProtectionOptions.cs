namespace Reserva.Api.Security
{
    public class ApiProtectionOptions
    {
        public bool RateLimitEnabled { get; set; } = true;
        public int RateLimitWindowSeconds { get; set; } = 10;
        public int RateLimitMaxRequests { get; set; } = 30;
        public int RateLimitBlockSeconds { get; set; } = 60;

        public bool InjectionDetectionEnabled { get; set; } = true;
        public int MaxPayloadLength { get; set; } = 100000;
    }
}
