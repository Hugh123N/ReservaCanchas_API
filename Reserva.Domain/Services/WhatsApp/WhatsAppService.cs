using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Reserva.Domain.Services.WhatsApp
{
    /// <summary>
    /// Implementación del servicio WhatsApp Cloud API
    /// Documentación: https://developers.facebook.com/docs/whatsapp/cloud-api
    /// </summary>
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly string _phoneNumberId;
        private readonly string _accessToken;
        private readonly string _apiVersion;
        private readonly bool _isEnabled;

        public WhatsAppService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<WhatsAppService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Cargar configuración
            _phoneNumberId = _configuration["WhatsApp:PhoneNumberId"] ?? "";
            _accessToken = _configuration["WhatsApp:AccessToken"] ?? "";
            _apiVersion = _configuration["WhatsApp:ApiVersion"] ?? "v18.0";
            _isEnabled = _configuration.GetValue<bool>("WhatsApp:Enabled");

            // Configurar HttpClient
            _httpClient.BaseAddress = new Uri($"https://graph.facebook.com/{_apiVersion}/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SendTextMessageAsync(string phoneNumber, string message)
        {
            if (!_isEnabled)
            {
                _logger.LogWarning("WhatsApp está deshabilitado en la configuración. No se enviará el mensaje.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_phoneNumberId) || string.IsNullOrWhiteSpace(_accessToken))
            {
                _logger.LogError("WhatsApp no está configurado correctamente. Faltan PhoneNumberId o AccessToken.");
                return false;
            }

            try
            {
                // Limpiar número de teléfono (quitar espacios, guiones, etc.)
                var cleanPhone = CleanPhoneNumber(phoneNumber);

                if (string.IsNullOrEmpty(cleanPhone))
                {
                    _logger.LogWarning("Número de teléfono inválido: {PhoneNumber}", phoneNumber);
                    return false;
                }

                var payload = new
                {
                    messaging_product = "whatsapp",
                    recipient_type = "individual",
                    to = cleanPhone,
                    type = "text",
                    text = new
                    {
                        preview_url = false,
                        body = message
                    }
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_phoneNumberId}/messages", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation(
                        "Mensaje de WhatsApp enviado exitosamente a {PhoneNumber}. Response: {Response}",
                        cleanPhone,
                        responseBody);
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "Error al enviar mensaje de WhatsApp a {PhoneNumber}. Status: {StatusCode}, Error: {Error}",
                        cleanPhone,
                        response.StatusCode,
                        errorBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Excepción al enviar mensaje de WhatsApp a {PhoneNumber}",
                    phoneNumber);
                return false;
            }
        }

        public async Task<int> SendBulkTextMessageAsync(List<string> phoneNumbers, string message)
        {
            if (!_isEnabled)
            {
                _logger.LogWarning("WhatsApp está deshabilitado en la configuración. No se enviarán mensajes.");
                return 0;
            }

            if (phoneNumbers == null || !phoneNumbers.Any())
            {
                _logger.LogWarning("No se proporcionaron números de teléfono para envío masivo.");
                return 0;
            }

            int successCount = 0;

            foreach (var phoneNumber in phoneNumbers)
            {
                var sent = await SendTextMessageAsync(phoneNumber, message);
                if (sent)
                {
                    successCount++;
                }

                // Pequeña pausa entre mensajes para evitar rate limiting
                await Task.Delay(500);
            }

            _logger.LogInformation(
                "Envío masivo completado. {Success}/{Total} mensajes enviados exitosamente.",
                successCount,
                phoneNumbers.Count);

            return successCount;
        }

        /// <summary>
        /// Limpia el número de teléfono dejando solo dígitos y el signo +
        /// </summary>
        private string CleanPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            // Quitar espacios, guiones, paréntesis, etc.
            var cleaned = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Si no tiene +, agregar +51 (código de Perú) como default
            if (!cleaned.StartsWith("+"))
            {
                cleaned = "+51" + cleaned;
            }

            return cleaned;
        }
    }
}
