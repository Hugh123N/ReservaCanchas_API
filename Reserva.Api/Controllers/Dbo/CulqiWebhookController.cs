using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using System.Text.Json;

namespace Reserva.Api.Controllers.Dbo
{
    /// <summary>
    /// Controlador para recibir notificaciones de Culqi mediante webhooks
    /// </summary>
    [ApiController]
    [Route("api/culqi")]
    public class CulqiWebhookController : ControllerBase
    {
        private readonly IRepository<Pago> _pagoRepository;
        private readonly IRepository<EstadoPago> _estadoPagoRepository;
        private readonly CulqiService _culqiService;
        private readonly ILogger<CulqiWebhookController> _logger;

        public CulqiWebhookController(
            IRepository<Pago> pagoRepository,
            IRepository<EstadoPago> estadoPagoRepository,
            CulqiService culqiService,
            ILogger<CulqiWebhookController> logger)
        {
            _pagoRepository = pagoRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _culqiService = culqiService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para recibir notificaciones de eventos de Culqi
        /// URL a configurar en Culqi Panel: https://tudominio.com/api/culqi/webhook
        /// </summary>
        /// <returns>200 OK si se procesó correctamente</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            try
            {
                // Leer el cuerpo del webhook
                using var reader = new StreamReader(Request.Body);
                var webhookBody = await reader.ReadToEndAsync();

                _logger.LogInformation("Webhook recibido de Culqi: {Body}", webhookBody);

                // Validar firma del webhook (si Culqi proporciona)
                var signature = Request.Headers["X-Culqi-Signature"].FirstOrDefault();
                if (!string.IsNullOrEmpty(signature))
                {
                    if (!_culqiService.ValidateWebhookSignature(webhookBody, signature))
                    {
                        _logger.LogWarning("Firma del webhook inválida");
                        return Unauthorized(new { message = "Firma inválida" });
                    }
                }

                // Deserializar el evento
                var webhookEvent = JsonSerializer.Deserialize<CulqiWebhookEvent>(webhookBody);
                if (webhookEvent == null)
                {
                    _logger.LogError("Error al deserializar el webhook");
                    return BadRequest(new { message = "Formato de webhook inválido" });
                }

                _logger.LogInformation("Evento de Culqi recibido - Tipo: {Type}, ID: {Id}",
                    webhookEvent.Type, webhookEvent.Data.Id);

                // Procesar el evento según su tipo
                await ProcessWebhookEvent(webhookEvent);

                return Ok(new { message = "Webhook procesado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar webhook de Culqi");
                // Devolver 200 para evitar reintentos de Culqi si el error no es recuperable
                return Ok(new { message = "Error al procesar webhook, pero recibido" });
            }
        }

        /// <summary>
        /// Procesa un evento de webhook según su tipo
        /// </summary>
        private async Task ProcessWebhookEvent(CulqiWebhookEvent webhookEvent)
        {
            switch (webhookEvent.Type)
            {
                case "charge.succeeded":
                    await HandleChargeSucceeded(webhookEvent.Data);
                    break;

                case "charge.failed":
                    await HandleChargeFailed(webhookEvent.Data);
                    break;

                case "order.status.changed":
                    await HandleOrderStatusChanged(webhookEvent.Data);
                    break;

                default:
                    _logger.LogInformation("Evento de Culqi no manejado: {Type}", webhookEvent.Type);
                    break;
            }
        }

        /// <summary>
        /// Maneja el evento de pago exitoso (charge.succeeded)
        /// </summary>
        private async Task HandleChargeSucceeded(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago exitoso - ChargeId: {ChargeId}", data.Id);

            // Buscar el pago por CulqiChargeId
            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para ChargeId: {ChargeId}", data.Id);
                return;
            }

            // Verificar si ya está pagado
            if (pago.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado)
            {
                _logger.LogInformation("Pago {IdPago} ya estaba marcado como pagado", pago.IdPago);
                return;
            }

            // Actualizar estado a Pagado
            var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Pagado
            );

            if (estadoPagado != null)
            {
                pago.IdEstadoPago = estadoPagado.IdEstadoPago;
                pago.CulqiReferenceCode = data.ReferenceCode;
                pago.CodigoOperacion = data.ReferenceCode; // Usar reference_code como código de operación
                pago.MontoPendiente = 0;

                await _pagoRepository.UpdateAsync(pago);
                await _pagoRepository.SaveAsync();

                _logger.LogInformation("Pago {IdPago} actualizado a estado PAGADO exitosamente", pago.IdPago);
            }
        }

        /// <summary>
        /// Maneja el evento de pago fallido (charge.failed)
        /// </summary>
        private async Task HandleChargeFailed(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago fallido - ChargeId: {ChargeId}", data.Id);

            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para ChargeId: {ChargeId}", data.Id);
                return;
            }

            // Actualizar estado a Rechazado
            var estadoRechazado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Rechazado
            );

            if (estadoRechazado != null)
            {
                pago.IdEstadoPago = estadoRechazado.IdEstadoPago;

                await _pagoRepository.UpdateAsync(pago);
                await _pagoRepository.SaveAsync();

                _logger.LogInformation("Pago {IdPago} actualizado a estado RECHAZADO", pago.IdPago);
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de estado de orden (order.status.changed)
        /// Este evento se usa para pagos con QR (Yape, Plin, billeteras móviles)
        /// </summary>
        private async Task HandleOrderStatusChanged(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando cambio de estado de orden - OrderId: {OrderId}, Estado: {State}",
                data.Id, data.State);

            // Buscar el pago asociado a esta orden
            // Nota: La búsqueda depende de cómo se almacene la referencia de la orden
            // Si se usa metadata, buscar por metadata en lugar de CulqiChargeId
            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id || p.NumeroReferencia == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para OrderId: {OrderId}", data.Id);
                return;
            }

            // Verificar el estado de la orden
            if (data.State == "paid" || data.State == "paid_out")
            {
                // Orden pagada exitosamente
                var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pagado
                );

                if (estadoPagado != null)
                {
                    pago.IdEstadoPago = estadoPagado.IdEstadoPago;
                    pago.CulqiReferenceCode = data.ReferenceCode;
                    pago.MontoPendiente = 0;

                    await _pagoRepository.UpdateAsync(pago);
                    await _pagoRepository.SaveAsync();

                    _logger.LogInformation("Pago {IdPago} actualizado a PAGADO por orden", pago.IdPago);
                }
            }
            else if (data.State == "expired" || data.State == "deleted")
            {
                // Orden expirada o cancelada
                var estadoRechazado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Rechazado
                );

                if (estadoRechazado != null)
                {
                    pago.IdEstadoPago = estadoRechazado.IdEstadoPago;

                    await _pagoRepository.UpdateAsync(pago);
                    await _pagoRepository.SaveAsync();

                    _logger.LogInformation("Pago {IdPago} marcado como RECHAZADO (orden {State})",
                        pago.IdPago, data.State);
                }
            }
        }

        /// <summary>
        /// Endpoint de prueba para verificar que el webhook está funcionando
        /// </summary>
        [HttpGet("webhook/test")]
        public IActionResult TestWebhook()
        {
            return Ok(new
            {
                message = "Webhook de Culqi funcionando correctamente",
                timestamp = DateTimeOffset.UtcNow
            });
        }
    }
}
