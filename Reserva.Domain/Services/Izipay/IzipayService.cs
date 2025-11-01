using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Reserva.Domain.Services.Izipay
{
    /// <summary>
    /// Servicio para integración con Izipay API
    /// Documentación: https://developers.izipay.pe/
    /// </summary>
    public class IzipayService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly string _hmacKey;

        public IzipayService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;

            _apiUrl = _configuration.GetValue<string>("Izipay:ApiUrl")
                ?? "https://api.micuentaweb.pe/api-payment/V4/Charge";
            _username = _configuration.GetValue<string>("Izipay:Username")
                ?? throw new InvalidOperationException("Izipay:Username no está configurado");
            _password = _configuration.GetValue<string>("Izipay:Password")
                ?? throw new InvalidOperationException("Izipay:Password no está configurado");
            _hmacKey = _configuration.GetValue<string>("Izipay:HmacSha256Key")
                ?? throw new InvalidOperationException("Izipay:HmacSha256Key no está configurado");
        }

        /// <summary>
        /// Crea un pago en Izipay y obtiene el FormToken para mostrar en el frontend
        /// </summary>
        public async Task<IzipayCreatePaymentResponse> CreatePaymentAsync(IzipayCreatePaymentRequest request)
        {
            try
            {
                var url = $"{_apiUrl}/CreatePayment";

                // Configurar autenticación Basic Auth
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", authToken);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync(url, content);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new IzipayCreatePaymentResponse
                    {
                        Status = "ERROR",
                        ErrorMessage = $"Error HTTP {httpResponse.StatusCode}: {responseBody}",
                        ErrorCode = httpResponse.StatusCode.ToString()
                    };
                }

                var response = JsonSerializer.Deserialize<IzipayCreatePaymentResponse>(responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                return response ?? new IzipayCreatePaymentResponse
                {
                    Status = "ERROR",
                    ErrorMessage = "Respuesta vacía de Izipay"
                };
            }
            catch (Exception ex)
            {
                return new IzipayCreatePaymentResponse
                {
                    Status = "ERROR",
                    ErrorMessage = $"Excepción al llamar a Izipay: {ex.Message}",
                    ErrorCode = "EXCEPTION"
                };
            }
        }

        /// <summary>
        /// Valida la firma HMAC-SHA256 de un webhook de Izipay
        /// Esto asegura que la notificación realmente viene de Izipay
        /// </summary>
        public bool ValidateWebhookSignature(string payload, string receivedSignature)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(_hmacKey);
                var payloadBytes = Encoding.UTF8.GetBytes(payload);

                using (var hmac = new HMACSHA256(keyBytes))
                {
                    var hashBytes = hmac.ComputeHash(payloadBytes);
                    var computedSignature = Convert.ToBase64String(hashBytes);

                    // Comparación segura contra timing attacks
                    return CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(computedSignature),
                        Encoding.UTF8.GetBytes(receivedSignature)
                    );
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Consulta el estado de una transacción en Izipay
        /// Útil para verificar pagos manualmente
        /// </summary>
        public async Task<IzipayTransactionStatus?> GetTransactionStatusAsync(string transactionId)
        {
            try
            {
                var url = $"{_apiUrl}/Get/{transactionId}";

                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", authToken);

                var httpResponse = await _httpClient.GetAsync(url);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var response = JsonSerializer.Deserialize<IzipayTransactionStatus>(responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                return response;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convierte monto decimal a céntimos (formato requerido por Izipay)
        /// Ejemplo: 50.00 → 5000
        /// </summary>
        public int ConvertToCents(decimal amount)
        {
            return (int)(amount * 100);
        }

        /// <summary>
        /// Convierte céntimos a decimal
        /// Ejemplo: 5000 → 50.00
        /// </summary>
        public decimal ConvertFromCents(int cents)
        {
            return cents / 100m;
        }
    }

    /// <summary>
    /// Estado de una transacción en Izipay
    /// </summary>
    public class IzipayTransactionStatus
    {
        public string TransactionId { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string? PaymentMethod { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
