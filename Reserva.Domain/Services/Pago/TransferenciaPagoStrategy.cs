using Microsoft.Extensions.Configuration;
using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Transferencia mediante proveedor externo (MercadoPago, etc.)
    /// PLACEHOLDER: Preparado para integración futura con pasarelas de pago
    /// </summary>
    public class TransferenciaPagoStrategy : IPagoStrategy
    {
        private readonly IConfiguration _configuration;

        public TransferenciaPagoStrategy(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObtenerCodigoMetodoPago()
        {
            return Common.Constants.METODO_PAGO.Transferencia;
        }

        public async Task<PagoStrategyResult> ProcesarPagoAsync(
            Entity.Pago pago,
            Cancha cancha,
            Entity.Reserva reserva)
        {
            // ========================================
            // 🚧 IMPLEMENTACIÓN FUTURA: MERCADOPAGO
            // ========================================
            //
            // TODO: Integrar con MercadoPago (o proveedor similar)
            //
            // Pasos pendientes:
            // 1. Instalar SDK: dotnet add package MercadoPago
            // 2. Crear Preference con datos del pago
            // 3. Generar init_point (URL de pago)
            // 4. Devolver URL al cliente
            // 5. Implementar webhook para recibir notificaciones
            //
            // Código ejemplo:
            //
            // var preference = new PreferenceRequest
            // {
            //     Items = new List<PreferenceItemRequest>
            //     {
            //         new PreferenceItemRequest
            //         {
            //             Title = $"Reserva {cancha.Nombre}",
            //             Quantity = 1,
            //             CurrencyId = "PEN",
            //             UnitPrice = pago.Monto,
            //         }
            //     },
            //     ExternalReference = pago.IdPago.ToString(),
            //     BackUrls = new PreferenceBackUrlsRequest
            //     {
            //         Success = _configuration["MercadoPago:SuccessUrl"],
            //         Failure = _configuration["MercadoPago:FailureUrl"]
            //     },
            //     NotificationUrl = _configuration["MercadoPago:WebhookUrl"]
            // };
            //
            // var client = new PreferenceClient();
            // Preference result = await client.CreateAsync(preference);
            // string paymentUrl = result.InitPoint;
            //
            // ========================================

            // Por ahora, devolver mensaje de no implementado
            return await Task.FromResult(new PagoStrategyResult
            {
                RequiereConfirmacion = true,
                InformacionAdicional = "⚠️ MÉTODO DE PAGO NO IMPLEMENTADO AÚN.\n\n" +
                    $"Se requiere configurar la integración con MercadoPago u otro proveedor de pagos.\n" +
                    $"Monto a pagar: S/ {pago.Monto:F2}\n\n" +
                    "Ver FLUJOS_PAGO.md para instrucciones de implementación."
            });
        }

        // ========================================
        // MÉTODOS PARA IMPLEMENTACIÓN FUTURA
        // ========================================

        /// <summary>
        /// Crea una preferencia de pago en MercadoPago (PENDIENTE)
        /// </summary>
        private async Task<string> CrearPreferenciaMercadoPago(Entity.Pago pago, Cancha cancha, Entity.Reserva reserva)
        {
            // TODO: Implementar creación de preference
            await Task.CompletedTask;
            throw new NotImplementedException("MercadoPago no configurado");
        }

        /// <summary>
        /// Procesa el webhook de MercadoPago cuando se confirma un pago (PENDIENTE)
        /// Este método debería estar en un PagoWebhookController
        /// </summary>
        private async Task ProcesarWebhookMercadoPago(string paymentId)
        {
            // TODO: Implementar procesamiento de webhook
            // 1. Verificar firma del webhook
            // 2. Consultar estado del pago en MercadoPago
            // 3. Actualizar estado del pago en BD
            // 4. Actualizar estado de reserva si pago aprobado
            await Task.CompletedTask;
            throw new NotImplementedException("Webhook MercadoPago no configurado");
        }
    }
}

// ========================================
// CONFIGURACIÓN REQUERIDA (appsettings.json)
// ========================================
//
// {
//   "MercadoPago": {
//     "AccessToken": "APP_USR-xxxx-xxxx-xxxx-xxxx",
//     "PublicKey": "APP_USR-xxxx-xxxx-xxxx",
//     "WebhookUrl": "https://tuapp.com/api/Pago/webhook-mercadopago",
//     "SuccessUrl": "https://tuapp.com/pago-exitoso",
//     "FailureUrl": "https://tuapp.com/pago-fallido"
//   }
// }
//
// ========================================
