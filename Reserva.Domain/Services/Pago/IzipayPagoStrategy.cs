using Microsoft.Extensions.Configuration;
using Reserva.Domain.Services.Izipay;
using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Yape y Plin mediante Izipay
    /// Reemplaza las estrategias manuales anteriores con integración real
    /// </summary>
    public class IzipayPagoStrategy : IPagoStrategy
    {
        private readonly IConfiguration _configuration;
        private readonly IzipayService _izipayService;
        private readonly string _metodoPago; // "YAPE" o "PLIN"

        public IzipayPagoStrategy(
            IConfiguration configuration,
            IzipayService izipayService,
            string metodoPago)
        {
            _configuration = configuration;
            _izipayService = izipayService;
            _metodoPago = metodoPago;
        }

        public async Task<PagoStrategyResult> ProcesarPagoAsync(
            Entity.Pago pago,
            Cancha cancha,
            Entity.Reserva reserva)
        {
            try
            {
                // 1. Preparar datos para Izipay
                string orderId = $"RESERVA-{reserva.IdReserva}";
                string description = GenerarDescripcion(cancha, reserva);

                var request = new IzipayCreatePaymentRequest
                {
                    Amount = _izipayService.ConvertToCents(pago.Monto),
                    Currency = pago.Moneda,
                    OrderId = orderId,
                    Description = description,
                    PaymentMethodType = _metodoPago, // "YAPE" o "PLIN"
                    Customer = new IzipayCustomer
                    {
                        Email = reserva.IdUsuarioNavigation?.Email ?? "noemail@reserva.com",
                        Reference = reserva.IdUsuario.ToString(),
                        BillingDetails = reserva.IdUsuarioNavigation?.UserName
                    }
                };

                // 2. Llamar a Izipay API para crear el pago
                var izipayResponse = await _izipayService.CreatePaymentAsync(request);

                if (izipayResponse.Status != "SUCCESS" || izipayResponse.Answer == null)
                {
                    // Error al crear el pago en Izipay
                    return new PagoStrategyResult
                    {
                        RequiereConfirmacion = true,
                        InformacionAdicional = $"⚠️ Error al procesar pago con Izipay.\n\n" +
                            $"Error: {izipayResponse.ErrorMessage}\n" +
                            $"Código: {izipayResponse.ErrorCode}\n\n" +
                            $"Por favor intenta nuevamente o contacta al administrador."
                    };
                }

                // 3. Guardar datos de Izipay en el objeto Pago
                // Nota: Estos se guardarán en la base de datos en el CommandHandler
                pago.IzipayTransactionId = izipayResponse.Answer.TransactionId;
                pago.IzipayFormToken = izipayResponse.Answer.FormToken;
                pago.IzipayPaymentUrl = izipayResponse.Answer.PaymentUrl
                    ?? GenerarPaymentUrl(izipayResponse.Answer.FormToken);

                // 4. Obtener URLs de retorno
                string successUrl = _configuration.GetValue<string>("Izipay:SuccessUrl")
                    ?? "https://tuapp.com/pago-exitoso";
                string failureUrl = _configuration.GetValue<string>("Izipay:FailureUrl")
                    ?? "https://tuapp.com/pago-fallido";

                // 5. Retornar resultado con datos para el frontend
                return new PagoStrategyResult
                {
                    // Datos específicos de Izipay
                    IzipayFormToken = izipayResponse.Answer.FormToken,
                    IzipayPaymentUrl = pago.IzipayPaymentUrl,
                    IzipayTransactionId = izipayResponse.Answer.TransactionId,

                    RequiereConfirmacion = true,
                    InformacionAdicional = GenerarInstrucciones(_metodoPago, pago.Monto)
                };
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción inesperada
                return new PagoStrategyResult
                {
                    RequiereConfirmacion = true,
                    InformacionAdicional = $"⚠️ Error inesperado al procesar el pago.\n\n" +
                        $"Detalles técnicos: {ex.Message}\n\n" +
                        $"Por favor contacta al administrador."
                };
            }
        }

        private string GenerarDescripcion(Cancha cancha, Entity.Reserva reserva)
        {
            var horas = reserva.ReservaDetalle
                .OrderBy(d => d.HoraInicio)
                .Select(d => d.HoraInicio.ToString(@"hh\:mm"));

            var horasTexto = string.Join(", ", horas);

            return $"Reserva {cancha.Nombre} - {reserva.Fecha:dd/MM/yyyy} {horasTexto}";
        }

        private string GenerarPaymentUrl(string formToken)
        {
            // URL de ejemplo - ajustar según documentación de Izipay
            string baseUrl = _configuration.GetValue<string>("Izipay:PaymentPageUrl")
                ?? "https://secure.micuentaweb.pe/payment";

            return $"{baseUrl}?formToken={formToken}";
        }

        private string GenerarInstrucciones(string metodoPago, decimal monto)
        {
            string metodoNombre = metodoPago == "YAPE" ? "Yape" : "Plin";

            return $@"✅ PAGO CON {metodoNombre.ToUpper()} - IZIPAY

Monto a pagar: S/ {monto:F2}

📱 INSTRUCCIONES:

1. En la siguiente pantalla verás un código QR oficial de {metodoNombre}

2. Abre tu app {metodoNombre} en tu celular

3. Escanea el código QR que aparece

4. La app {metodoNombre} abrirá automáticamente con el monto pre-cargado

5. Confirma el pago en tu app

6. ¡Listo! Tu reserva se confirmará automáticamente

⏱️ El pago es procesado en tiempo real por Izipay.
✅ Recibirás confirmación inmediata cuando se complete.

Método de pago seguro certificado por Izipay.";
        }
    }
}
