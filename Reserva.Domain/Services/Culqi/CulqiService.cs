using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Servicio para integración con la API de Culqi
    /// Maneja creación de cargos y suscripciones
    /// Implementa ICulqiService para inyección de dependencias
    /// </summary>
    public class CulqiService : ICulqiService
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

            _secretKey = _configuration["Culqi:SecretKey"] ?? throw new InvalidOperationException("Culqi:SecretKey no configurado");
            _apiBaseUrl = _configuration["Culqi:ApiBaseUrl"] ?? throw new InvalidOperationException("Culqi:ApiBaseUrl no configurado");

            // Configurar HttpClient con autenticación Bearer
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Cargos Únicos (Mantenido para otros usos)

        /// <summary>
        /// Crea un cargo (charge) en Culqi - Para pagos únicos
        /// </summary>
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

        #endregion

        #region Suscripciones (Para Planes SaaS)

        /// <summary>
        /// Crea una suscripción en Culqi para cobros recurrentes
        /// </summary>
        public async Task<CulqiSubscriptionResponse> CreateSubscriptionAsync(CulqiCreateSubscriptionRequest request)
        {
            try
            {
                _logger.LogInformation("Creando suscripción en Culqi - Plan: {PlanId}, Customer: {CustomerId}",
                    request.PlanId, request.CustomerId);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v2/subscriptions", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear suscripción en Culqi. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error al crear suscripción",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var subscriptionResponse = JsonSerializer.Deserialize<CulqiSubscriptionResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Suscripción creada exitosamente - SubscriptionId: {SubscriptionId}",
                    subscriptionResponse.Id);

                return subscriptionResponse;
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
        /// Cancela una suscripción en Culqi
        /// </summary>
        public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
        {
            try
            {
                _logger.LogInformation("Cancelando suscripción en Culqi - SubscriptionId: {SubscriptionId}", subscriptionId);

                var response = await _httpClient.DeleteAsync($"/v2/subscriptions/{subscriptionId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al cancelar suscripción. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);
                    return false;
                }

                _logger.LogInformation("Suscripción cancelada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar suscripción");
                return false;
            }
        }

        /// <summary>
        /// Obtiene los detalles de una suscripción
        /// </summary>
        public async Task<CulqiSubscriptionResponse?> GetSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v2/subscriptions/{subscriptionId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Suscripción no encontrada: {SubscriptionId}", subscriptionId);
                    return null;
                }

                return JsonSerializer.Deserialize<CulqiSubscriptionResponse>(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener suscripción");
                return null;
            }
        }

        /// <summary>
        /// Actualiza una suscripción en Culqi (ej: cambiar plan con prorrateo)
        /// </summary>
        public async Task<CulqiSubscriptionResponse?> UpdateSubscriptionAsync(string subscriptionId, CulqiUpdateSubscriptionRequest request)
        {
            try
            {
                _logger.LogInformation("Actualizando suscripción en Culqi - SubscriptionId: {SubscriptionId}, NewPlanId: {PlanId}",
                    subscriptionId, request.PlanId);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/v2/subscriptions/{subscriptionId}")
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al actualizar suscripción. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error al actualizar suscripción",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var subscriptionResponse = JsonSerializer.Deserialize<CulqiSubscriptionResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Suscripción actualizada exitosamente - SubscriptionId: {SubscriptionId}",
                    subscriptionResponse.Id);

                return subscriptionResponse;
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

        #endregion

        #region Planes (Para Suscripciones)

        /// <summary>
        /// Crea un plan en Culqi (necesario para suscripciones)
        /// </summary>
        public async Task<CulqiPlanResponse> CreatePlanAsync(CulqiCreatePlanRequest request)
        {
            try
            {
                _logger.LogInformation("Creando plan en Culqi - ID: {PlanId}, Nombre: {Name}",
                    request.Id, request.Name);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v2/plans", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear plan en Culqi. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error al crear plan",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var planResponse = JsonSerializer.Deserialize<CulqiPlanResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Plan creado exitosamente - PlanId: {PlanId}", planResponse.Id);

                return planResponse;
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
        /// Obtiene un plan de Culqi por su ID
        /// </summary>
        public async Task<CulqiPlanResponse?> GetPlanAsync(string planId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v2/plans/{planId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Plan no encontrado: {PlanId}", planId);
                    return null;
                }

                return JsonSerializer.Deserialize<CulqiPlanResponse>(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plan");
                return null;
            }
        }

        #endregion

        #region Clientes (Para Suscripciones)

        /// <summary>
        /// Crea un cliente en Culqi (necesario para suscripciones)
        /// </summary>
        public async Task<CulqiCustomerResponse> CreateCustomerAsync(CulqiCreateCustomerRequest request)
        {
            try
            {
                _logger.LogInformation("Creando cliente en Culqi - Email: {Email}", request.Email);

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v2/customers", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear cliente en Culqi. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error al crear cliente",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var customerResponse = JsonSerializer.Deserialize<CulqiCustomerResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Cliente creado exitosamente - CustomerId: {CustomerId}", customerResponse.Id);

                return customerResponse;
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
        /// Obtiene un cliente de Culqi por su ID
        /// </summary>
        public async Task<CulqiCustomerResponse?> GetCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v2/customers/{customerId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Cliente no encontrado: {CustomerId}", customerId);
                    return null;
                }

                return JsonSerializer.Deserialize<CulqiCustomerResponse>(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente");
                return null;
            }
        }

        #endregion

        #region Tarjetas (Cards)

        /// <summary>
        /// Obtiene la tarjeta de un cliente en Culqi
        /// </summary>
        public async Task<CulqiCardResponse?> GetCardAsync(string customerId)
        {
            try
            {
                _logger.LogInformation("Obteniendo tarjeta del cliente - CustomerId: {CustomerId}", customerId);

                var response = await _httpClient.GetAsync($"/v2/customers/{customerId}/cards");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("No se pudo obtener tarjeta del cliente: {CustomerId}", customerId);
                    return null;
                }

                var cardsResponse = JsonSerializer.Deserialize<CulqiCardsListResponse>(responseContent);
                return cardsResponse?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarjeta del cliente");
                return null;
            }
        }

        /// <summary>
        /// Crea una tarjeta en Culqi asociada a un cliente
        /// </summary>
        public async Task<CulqiCardResponse> CreateCardAsync(string customerId, string tokenId)
        {
            try
            {
                _logger.LogInformation("Creando tarjeta en Culqi - CustomerId: {CustomerId}", customerId);

                var request = new CulqiCreateCardRequest
                {
                    TokenId = tokenId,
                    CustomerId = customerId,
                    Validate = true
                };

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/v2/cards", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear tarjeta en Culqi. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorResponse = JsonSerializer.Deserialize<CulqiErrorResponse>(responseContent);
                    throw new CulqiException(
                        errorResponse?.MerchantMessage ?? "Error al crear tarjeta",
                        errorResponse?.UserMessage,
                        errorResponse?.Code
                    );
                }

                var cardResponse = JsonSerializer.Deserialize<CulqiCardResponse>(responseContent)
                    ?? throw new CulqiException("Respuesta inválida de Culqi");

                _logger.LogInformation("Tarjeta creada exitosamente - CardId: {CardId}", cardResponse.Id);

                return cardResponse;
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
        /// Elimina una tarjeta de Culqi
        /// </summary>
        public async Task<bool> DeleteCardAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Eliminando tarjeta de Culqi - CardId: {CardId}", cardId);

                var response = await _httpClient.DeleteAsync($"/v2/cards/{cardId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al eliminar tarjeta. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);
                    return false;
                }

                _logger.LogInformation("Tarjeta eliminada exitosamente - CardId: {CardId}", cardId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarjeta");
                return false;
            }
        }

        #endregion

        #region Métodos Helper

        /// <summary>
        /// Convierte un monto decimal en soles a centavos (entero)
        /// </summary>
        public static int ConvertToCents(decimal amount)
        {
            return (int)(amount * 100);
        }

        /// <summary>
        /// Convierte un monto en centavos a soles (decimal)
        /// </summary>
        public static decimal ConvertToSoles(int cents)
        {
            return cents / 100m;
        }

        /// <summary>
        /// Valida la firma de un webhook de Culqi
        /// NOTA: Culqi no documenta públicamente el método de validación de webhooks.
        /// Esta implementación debe actualizarse según la documentación oficial de Culqi.
        /// </summary>
        public bool ValidateWebhookSignature(string payload, string signature)
        {
            // TODO: Implementar validación de firma cuando Culqi proporcione la documentación
            // Por ahora, validamos solo que exista el payload
            _logger.LogWarning("Validación de firma de webhook no implementada - Culqi no documenta el método");
            return !string.IsNullOrEmpty(payload);
        }

        #endregion
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
