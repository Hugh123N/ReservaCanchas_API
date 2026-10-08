using Newtonsoft.Json;
using Reserva.Dto.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Services.Email
{
    internal class BrevoEmailService
    {
        private readonly string _apiKey;

        public BrevoEmailService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<ResponseDto> SendEmailAsync(BrevoEmailRequest emailRequest)
        {
            var response = new ResponseDto();
            using var httpClient = new HttpClient();

            try
            {
                var url = $"https://api.brevo.com/v3/smtp/email";
                var request = CreateRequest(HttpMethod.Post, url, emailRequest);

                using var httpResponse = await httpClient.SendAsync(request);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    response.AddErrorResult($"Error al enviar email: {httpResponse.StatusCode} - {errorContent}");
                    return response;
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BrevoEmailResponse>(responseContent);

                if (result != null && !string.IsNullOrEmpty(result.MessageId))
                {
                    response.AddOkResult($"Email enviado exitosamente. MessageId: {result.MessageId}");
                }
                else
                {
                    response.AddOkResult("Email enviado exitosamente");
                }

                return response;
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al enviar email con Brevo: {ex.Message}");
                return response;
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("api-key", _apiKey);

            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }

        // DTOs para Brevo API
        public class BrevoEmailRequest
        {
            [JsonProperty("sender")]
            public BrevoSender Sender { get; set; } = null!;

            [JsonProperty("to")]
            public List<BrevoRecipient> To { get; set; } = new();

            [JsonProperty("cc")]
            public List<BrevoRecipient>? Cc { get; set; }

            [JsonProperty("bcc")]
            public List<BrevoRecipient>? Bcc { get; set; }

            [JsonProperty("subject")]
            public string Subject { get; set; } = null!;

            [JsonProperty("htmlContent")]
            public string? HtmlContent { get; set; }

            [JsonProperty("textContent")]
            public string? TextContent { get; set; }
        }

        public class BrevoSender
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; } = null!;
        }

        public class BrevoRecipient
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; } = null!;
        }

        public class BrevoEmailResponse
        {
            [JsonProperty("messageId")]
            public string? MessageId { get; set; }
        }
    }
}
