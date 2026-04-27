using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Services.Culqi;
using Reserva.Domain.Services.Notificacion;
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
        private readonly IRepository<Entity.Pago> _pagoRepository;
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly CulqiService _culqiService;
        private readonly INotificacionService _notificacionService;
        private readonly ILogger<CulqiWebhookController> _logger;

        public CulqiWebhookController(
            IRepository<Entity.Pago> pagoRepository,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.AspNetUsers> userRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            CulqiService culqiService,
            INotificacionService notificacionService,
            ILogger<CulqiWebhookController> logger)
        {
            _pagoRepository = pagoRepository;
            _proveedorPlanRepository = proveedorPlanRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _userRepository = userRepository;
            _proveedorRepository = proveedorRepository;
            _culqiService = culqiService;
            _notificacionService = notificacionService;
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

            // Buscar primero en PagoPlan (planes de proveedores)
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation
            );

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                await HandlePlanPaymentSucceeded(proveedorPlan, proveedor, data);
                return;
            }

            // Buscar en Pago (reservas legacy)
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
                pago.CodigoOperacion = data.ReferenceCode;
                pago.MontoPendiente = 0;

                await _pagoRepository.UpdateAsync(pago);
                await _pagoRepository.SaveAsync();

                _logger.LogInformation("Pago {IdPago} actualizado a estado PAGADO exitosamente", pago.IdPago);
            }
        }

        private async Task HandlePlanPaymentSucceeded(ProveedorPlan proveedorPlan, Entity.Proveedor? proveedor, CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago de plan exitoso - ProveedorPlanId: {Id}", proveedorPlan.IdProveedorPlan);

            // Buscar el PagoPlan asociado
            var pagoPlanRepo = _proveedorPlanRepository;
            var pagoPlan = _proveedorPlanRepository.FindByAsync(
                pp => pp.IdProveedorPlan == proveedorPlan.IdProveedorPlan,
                pp => pp.PagoPlan
            ).Result.FirstOrDefault()?.PagoPlan?.FirstOrDefault();

            if (pagoPlan != null)
            {
                var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pagado
                );

                if (estadoPagado != null && false)
                {
                    pagoPlan.IdEstadoPago = estadoPagado.IdEstadoPago;
                    pagoPlan.CodigoOperacion = data.ReferenceCode;
                    pagoPlan.FechaPago = DateTimeOffset.UtcNow;
                }
            }

            // Activar el plan del proveedor
            proveedorPlan.Estado = "ACTIVE";
            proveedorPlan.FechaProximoCobro = null;
            proveedorPlan.GracePeriodHasta = null;
            proveedorPlan.CulqiSubscriptionId = data.Id;

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            // Notificar al proveedor
            var emailExitoso = proveedor?.IdUsuarioNavigation?.Email;
            if (!string.IsNullOrEmpty(emailExitoso))
            {
                await _notificacionService.NotificarRenovacionExitosaPlanAsync(
                    proveedorPlan,
                    proveedorPlan.IdPlaneNavigation!,
                    emailExitoso
                );
            }

            _logger.LogInformation("ProveedorPlan {Id} activado exitosamente", proveedorPlan.IdProveedorPlan);
        }

        /// <summary>
        /// Maneja el evento de pago fallido (charge.failed)
        /// </summary>
        private async Task HandleChargeFailed(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago fallido - ChargeId: {ChargeId}", data.Id);

            // Buscar primero en ProveedorPlan
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation
            );

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                await HandlePlanPaymentFailed(proveedorPlan, proveedor, data);
                return;
            }

            // Buscar en Pago
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

        private async Task HandlePlanPaymentFailed(ProveedorPlan proveedorPlan, Entity.Proveedor? proveedor, CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago de plan fallido - ProveedorPlanId: {Id}", proveedorPlan.IdProveedorPlan);

            // Cambiar estado a GRACE (periodo de gracia)
            proveedorPlan.Estado = "GRACE";
            proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            // Notificar al proveedor
            var emailFallo = proveedor?.IdUsuarioNavigation?.Email;
            if (!string.IsNullOrEmpty(emailFallo))
            {
                await _notificacionService.NotificarFalloPagoPlanAsync(
                    proveedorPlan,
                    proveedorPlan.IdPlaneNavigation!,
                    emailFallo
                );
            }

            _logger.LogInformation("ProveedorPlan {Id} cambiado a estado GRACE", proveedorPlan.IdProveedorPlan);
        }

        /// <summary>
        /// Maneja el evento de cambio de estado de orden (order.status.changed)
        /// Este evento se usa para pagos con QR (Yape, Plin, billeteras móviles)
        /// </summary>
        private async Task HandleOrderStatusChanged(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando cambio de estado de orden - OrderId: {OrderId}, Estado: {State}",
                data.Id, data.State);

            // Buscar primero en ProveedorPlan
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation
            );

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                if (data.State == "paid" || data.State == "paid_out")
                {
                    await HandlePlanPaymentSucceeded(proveedorPlan, proveedor, data);
                }
                else if (data.State == "expired" || data.State == "deleted")
                {
                    await HandlePlanPaymentFailed(proveedorPlan, proveedor, data);
                }
                return;
            }

            // Buscar en Pago
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
