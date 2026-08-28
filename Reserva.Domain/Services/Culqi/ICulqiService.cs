using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reserva.Domain.Services.Culqi
{
    /// <summary>
    /// Interface para el servicio de integración con Culqi
    /// Soporta pagos únicos (charges) y suscripciones recurrentes
    /// </summary>
    public interface ICulqiService
    {
        #region Pagos Únicos (Charges)

        /// <summary>
        /// Crea un cargo único en Culqi
        /// </summary>
        Task<CulqiChargeResponse> CreateChargeAsync(CulqiCreateChargeRequest request);

        #endregion

        #region Suscripciones (Para Planes SaaS)

        /// <summary>
        /// Crea una suscripción recurrente en Culqi
        /// </summary>
        Task<CulqiSubscriptionResponse> CreateSubscriptionAsync(CulqiCreateSubscriptionRequest request);

        /// <summary>
        /// Cancela una suscripción en Culqi
        /// </summary>
        Task<bool> CancelSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// Obtiene los detalles de una suscripción
        /// </summary>
        Task<CulqiSubscriptionResponse?> GetSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// Crea un plan en Culqi (necesario para suscripciones)
        /// </summary>
        Task<CulqiCreatePlanResponse> CreatePlanAsync(CulqiCreatePlanRequest request);

        /// <summary>
        /// Obtiene un plan de Culqi por su ID
        /// </summary>
        Task<CulqiPlanResponse?> GetPlanAsync(string planId);

        /// <summary>
        /// Crea un cliente en Culqi (necesario para suscripciones)
        /// </summary>
        Task<CulqiCustomerResponse> CreateCustomerAsync(CulqiCreateCustomerRequest request);

        /// <summary>
        /// Obtiene un cliente de Culqi por su ID
        /// </summary>
        Task<CulqiCustomerResponse?> GetCustomerAsync(string customerId);

        /// <summary>
        /// Actualiza una suscripción en Culqi (ej: cambiar plan con prorrateo)
        /// </summary>
        Task<CulqiSubscriptionResponse?> UpdateSubscriptionAsync(string subscriptionId, CulqiUpdateSubscriptionRequest request);

        #endregion

        #region Tarjetas (Cards)

        /// <summary>
        /// Obtiene la tarjeta de un cliente en Culqi
        /// </summary>
        Task<CulqiCardResponse?> GetCardAsync(string customerId);

        /// <summary>
        /// Crea una tarjeta en Culqi asociada a un cliente
        /// </summary>
        Task<CulqiCardResponse> CreateCardAsync(string customerId, string tokenId);

        /// <summary>
        /// Elimina una tarjeta de Culqi
        /// </summary>
        Task<bool> DeleteCardAsync(string cardId);

        #endregion

    #region Métodos Helper

    /// <summary>
    /// Convierte un monto decimal en soles a centavos
    /// </summary>
    static int ConvertToCents(decimal amount)
    {
        return (int)(amount * 100);
    }

    /// <summary>
    /// Convierte un monto en centavos a soles
    /// </summary>
    static decimal ConvertToSoles(int cents)
    {
        return cents / 100m;
    }

    /// <summary>
    /// Valida la firma de un webhook de Culqi
    /// </summary>
    bool ValidateWebhookSignature(string payload, string signature);

    #endregion
    }
}
