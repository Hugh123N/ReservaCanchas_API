using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Servicio para integración con la API de Culqi
    /// Maneja creación de cargos y validación de webhooks
    /// </summary>
    public class CulqiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CulqiService> _logger;
        private readonly string _secretKey;
        private readonly string _apiBaseUrl;

        public CulqiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CulqiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Obtener configuración de Culqi
            _secretKey = _configuration["Culqi:SecretKey"]
                ?? throw new InvalidOperationException("Culqi:SecretKey no configurado");

            _apiBaseUrl = _configuration["Culqi:ApiBaseUrl"] ?? "https://api.culqi.com";

            // Configurar HttpClient con autenticación Bearer
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Crea un cargo (charge) en Culqi
        /// </summary>
        /// <param name="request">Datos del cargo a crear</param>
        /// <returns>Respuesta de Culqi con los datos del cargo creado</returns>
        /// <exception cref="CulqiException">Si ocurre un error en la API de Culqi</exception>
        public async Task<CulqiChargeResponse> CreateChargeAsync(CulqiCreateChargeRequest request)
        {
            try
            {
                _logger.LogInformation("Creando cargo en Culqi - Monto: {Amount} centavos, Email: {Email}",
                    request.Amount, request.Email);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v2/charges", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear cargo en Culqi. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error desconocido al procesar el pago",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var chargeResponse = JsonSerializer.Deserialize<CulqiChargeResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Cargo creado exitosamente en Culqi - ChargeId: {ChargeId}",
                    chargeResponse.Id);

                return chargeResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al comunicarse con Culqi");
                throw new CulqiException("Error de conexión con el servicio de pagos", null, null, ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar la respuesta de Culqi");
                throw new CulqiException("Error al procesar la respuesta del servicio de pagos", null, null, ex);
            }
        }

        /// <summary>
        /// Convierte un monto decimal en soles a centavos (entero)
        /// </summary>
        /// <param name="amount">Monto en soles (ej: 45.50)</param>
        /// <returns>Monto en centavos (ej: 4550)</returns>
        public static int ConvertToCents(decimal amount)
        {
            return (int)(amount * 100);
        }

        /// <summary>
        /// Convierte un monto en centavos a soles (decimal)
        /// </summary>
        /// <param name="cents">Monto en centavos (ej: 4550)</param>
        /// <returns>Monto en soles (ej: 45.50)</returns>
        public static decimal ConvertToSoles(int cents)
        {
            return cents / 100m;
        }

        /// <summary>
        /// Valida la firma de un webhook de Culqi
        /// NOTA: Culqi no documenta públicamente el método de validación de webhooks.
        /// Esta implementación debe actualizarse según la documentación oficial de Culqi.
        /// </summary>
        /// <param name="payload">Cuerpo del webhook</param>
        /// <param name="signature">Firma recibida en el header</param>
        /// <returns>True si la firma es válida</returns>
        public bool ValidateWebhookSignature(string payload, string signature)
        {
            // TODO: Implementar validación de firma cuando Culqi proporcione la documentación
            // Por ahora, validamos solo que exista el payload
            _logger.LogWarning("Validación de firma de webhook no implementada - Culqi no documenta el método");
            return !string.IsNullOrEmpty(payload);
        }
    }

    /// <summary>
    /// Excepción específica para errores de Culqi
    /// </summary>
    public class CulqiException : Exception
    {
        public string? UserMessage { get; }
        public string? ErrorCode { get; }

        public CulqiException(string merchantMessage, string? userMessage = null, string? errorCode = null, Exception? innerException = null)
            : base(merchantMessage, innerException)
        {
            UserMessage = userMessage;
            ErrorCode = errorCode;
        }
    }
}
