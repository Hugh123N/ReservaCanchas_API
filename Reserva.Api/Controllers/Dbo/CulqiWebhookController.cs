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
        private readonly IRepository<Entity.PagoPlan> _pagoRepository;
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly ICulqiService _culqiService;
        private readonly INotificacionService _notificacionService;
        private readonly ILogger<CulqiWebhookController> _logger;

        public CulqiWebhookController(
            IRepository<Entity.PagoPlan> pagoRepository,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.AspNetUsers> userRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
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
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var webhookBody = await reader.ReadToEndAsync();

                _logger.LogInformation("Webhook recibido de Culqi: {Body}", webhookBody);

                var signature = Request.Headers["X-Culqi-Signature"].FirstOrDefault();
                if (!string.IsNullOrEmpty(signature))
                {
                    if (!_culqiService.ValidateWebhookSignature(webhookBody, signature))
                    {
                        _logger.LogWarning("Firma del webhook inválida");
                        return Unauthorized(new { message = "Firma inválida" });
                    }
                }

                var webhookEvent = JsonSerializer.Deserialize<CulqiWebhookEvent>(webhookBody);
                if (webhookEvent == null)
                {
                    _logger.LogError("Error al deserializar el webhook");
                    return BadRequest(new { message = "Formato de webhook inválido" });
                }

                _logger.LogInformation("Evento de Culqi recibido - Tipo: {Type}, ID: {Id}",
                    webhookEvent.Type, webhookEvent.Data.Id);

                await ProcessWebhookEvent(webhookEvent);

                return Ok(new { message = "Webhook procesado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar webhook de Culqi");
                return Ok(new { message = "Error al procesar webhook, pero recibido" });
            }
        }

        private async Task ProcessWebhookEvent(CulqiWebhookEvent webhookEvent)
        {
            switch (webhookEvent.Type)
            {
                case "charge.creation.succeeded":
                    await HandleChargeSucceeded(webhookEvent.Data);
                    break;

                case "charge.creation.failed":
                    await HandleChargeFailed(webhookEvent.Data);
                    break;

                case "order.status.changed":
                    await HandleOrderStatusChanged(webhookEvent.Data);
                    break;

                case "subscription.created":
                case "subscription.updated":
                case "subscription.deleted":
                    await HandleSubscriptionEvent(webhookEvent.Data, webhookEvent.Type);
                    break;

                default:
                    _logger.LogInformation("Evento de Culqi no manejado: {Type}", webhookEvent.Type);
                    break;
            }
        }

        private async Task HandleChargeSucceeded(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago exitoso - ChargeId: {ChargeId}", data.Id);

            // Buscar ProveedorPlan por subscription ID (para suscripciones)
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation,
                pp => pp.PagoPlan
            );

            // Si no encuentra por subscription, buscar por metadata (para pagos únicos)
            if (proveedorPlan == null && data.Metadata != null)
            {
                if (data.Metadata.TryGetValue("proveedor_id", out var proveedorIdStr) &&
                    data.Metadata.TryGetValue("tipo", out var tipo) && tipo == "pago_unico")
                {
                    // Para pagos únicos, buscar el plan activo más reciente del proveedor
                    if (int.TryParse(proveedorIdStr, out var proveedorId))
                    {
                        proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                            pp => pp.IdProveedor == proveedorId
                                && pp.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE
                                && pp.Activo,
                            pp => pp.IdPlaneNavigation,
                            pp => pp.PagoPlan
                        );
                    }
                }
            }

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                await HandlePlanPaymentSucceeded(proveedorPlan, proveedor, data);
                return;
            }

            // Buscar pago por charge ID
            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para ChargeId: {ChargeId}", data.Id);
                return;
            }

            if (pago.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado)
            {
                _logger.LogInformation("Pago {IdPagoPlan} ya estaba marcado como pagado", pago.IdPagoPlan);
                return;
            }

            var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Pagado
            );

            if (estadoPagado != null)
            {
                pago.IdEstadoPago = estadoPagado.IdEstadoPago;
                pago.CodigoOperacion = data.ReferenceCode;

                await _pagoRepository.UpdateAsync(pago);
                await _pagoRepository.SaveAsync();

                _logger.LogInformation("Pago {IdPagoPlan} actualizado a estado PAGADO exitosamente", pago.IdPagoPlan);
            }
        }

        private async Task HandlePlanPaymentSucceeded(ProveedorPlan proveedorPlan, Entity.Proveedor? proveedor, CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago de plan exitoso - ProveedorPlanId: {Id}", proveedorPlan.IdProveedorPlan);

            var pagoPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.IdProveedorPlan == proveedorPlan.IdProveedorPlan,
                pp => pp.PagoPlan
            );

            var pagoPlanActual = pagoPlan?.PagoPlan?.FirstOrDefault();

            if (pagoPlanActual != null)
            {
                var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pagado
                );

                if (estadoPagado != null)
                {
                    pagoPlanActual.IdEstadoPago = estadoPagado.IdEstadoPago;
                    pagoPlanActual.CodigoOperacion = data.ReferenceCode;
                    pagoPlanActual.FechaPago = DateTimeOffset.UtcNow;
                    await _proveedorPlanRepository.UpdateAsync(pagoPlan!);
                    await _proveedorPlanRepository.SaveAsync();
                }
            }

            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.ACTIVE;
            proveedorPlan.FechaProximoCobro = proveedorPlan.FechaFin;
            proveedorPlan.GracePeriodHasta = null;
            proveedorPlan.CulqiSubscriptionId = data.Id;

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

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

        private async Task HandleChargeFailed(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago fallido - ChargeId: {ChargeId}", data.Id);

            // Buscar ProveedorPlan por subscription ID
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation,
                pp => pp.PagoPlan
            );

            // Si no encuentra por subscription, buscar por metadata (para pagos únicos)
            if (proveedorPlan == null && data.Metadata != null)
            {
                if (data.Metadata.TryGetValue("proveedor_id", out var proveedorIdStr) &&
                    data.Metadata.TryGetValue("tipo", out var tipo) && tipo == "pago_unico")
                {
                    if (int.TryParse(proveedorIdStr, out var proveedorId))
                    {
                        proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                            pp => pp.IdProveedor == proveedorId
                                && pp.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE
                                && pp.Activo,
                            pp => pp.IdPlaneNavigation,
                            pp => pp.PagoPlan
                        );
                    }
                }
            }

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                await HandlePlanPaymentFailed(proveedorPlan, proveedor, data);
                return;
            }

            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para ChargeId: {ChargeId}", data.Id);
                return;
            }

            var estadoRechazado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Rechazado
            );

            if (estadoRechazado != null)
            {
                pago.IdEstadoPago = estadoRechazado.IdEstadoPago;

                await _pagoRepository.UpdateAsync(pago);
                await _pagoRepository.SaveAsync();

                _logger.LogInformation("Pago {IdPagoPlan} actualizado a estado RECHAZADO", pago.IdPagoPlan);
            }
        }

        private async Task HandlePlanPaymentFailed(ProveedorPlan proveedorPlan, Entity.Proveedor? proveedor, CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando pago de plan fallido - ProveedorPlanId: {Id}", proveedorPlan.IdProveedorPlan);

            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.GRACE;
            proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

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

        private async Task HandleSubscriptionEvent(CulqiWebhookData data, string eventType)
        {
            _logger.LogInformation("Procesando evento de suscripción: {EventType} - SubscriptionId: {SubscriptionId}",
                eventType, data.Id);

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.Id,
                pp => pp.IdPlaneNavigation,
                pp => pp.PagoPlan
            );

            if (proveedorPlan == null)
            {
                _logger.LogWarning("ProveedorPlan no encontrado para SubscriptionId: {SubscriptionId}", data.Id);
                return;
            }

            var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                p => p.IdProveedor == proveedorPlan.IdProveedor,
                p => p.IdUsuarioNavigation
            );

            switch (eventType)
            {
                case "subscription.created":
                    _logger.LogInformation("Suscripción creada para ProveedorPlan {Id}", proveedorPlan.IdProveedorPlan);

                    if (data.Metadata != null && data.Metadata.TryGetValue("next_billing_date", out var nextBillingCreatedStr))
                    {
                        if (long.TryParse(nextBillingCreatedStr, out var nextBillingCreatedTimestamp))
                        {
                            proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(nextBillingCreatedTimestamp);
                        }
                    }
                    else if (data.NextBillingDate.HasValue)
                    {
                        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(data.NextBillingDate.Value);
                    }

                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();
                    break;

                case "subscription.updated":
                    if (data.Metadata != null && data.Metadata.TryGetValue("next_billing_date", out var nextBillingStr))
                    {
                        if (long.TryParse(nextBillingStr, out var nextBillingTimestamp))
                        {
                            proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(nextBillingTimestamp);
                            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                            await _proveedorPlanRepository.SaveAsync();
                        }
                    }
                    else if (data.NextBillingDate.HasValue)
                    {
                        proveedorPlan.FechaProximoCobro = DateTimeOffset.FromUnixTimeSeconds(data.NextBillingDate.Value);
                        await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                        await _proveedorPlanRepository.SaveAsync();
                    }
                    break;

                case "subscription.deleted":
                    // La suscripción fue cancelada en Culqi
                    // El plan permanece ACTIVE hasta FechaFin, pero con renovación cancelada
                    proveedorPlan.AutoRenovacion = false;
                    proveedorPlan.CancelAtPeriodEnd = true;
                    proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
                    proveedorPlan.MotivoCancelacion = "Cancelado en Culqi";
                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();
                    _logger.LogInformation("Suscripción cancelada en Culqi para ProveedorPlan {Id}. Plan permanece activo hasta {FechaFin}", 
                        proveedorPlan.IdProveedorPlan, proveedorPlan.FechaFin);
                    break;
            }
        }

        private async Task HandleOrderStatusChanged(CulqiWebhookData data)
        {
            _logger.LogInformation("Procesando cambio de estado de orden - OrderId: {OrderId}, Estado: {State}",
                data.Id, data.State);

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

            var pago = await _pagoRepository.GetByAsync(
                p => p.CulqiChargeId == data.Id,
                p => p.IdEstadoPagoNavigation
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago no encontrado para OrderId: {OrderId}", data.Id);
                return;
            }

            if (data.State == "paid" || data.State == "paid_out")
            {
                var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Pagado
                );

                if (estadoPagado != null)
                {
                    pago.IdEstadoPago = estadoPagado.IdEstadoPago;
                    pago.CodigoOperacion = data.ReferenceCode;

                    await _pagoRepository.UpdateAsync(pago);
                    await _pagoRepository.SaveAsync();

                    _logger.LogInformation("Pago {IdPagoPlan} actualizado a PAGADO por orden", pago.IdPagoPlan);
                }
            }
            else if (data.State == "expired" || data.State == "deleted")
            {
                var estadoRechazado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Rechazado
                );

                if (estadoRechazado != null)
                {
                    pago.IdEstadoPago = estadoRechazado.IdEstadoPago;

                    await _pagoRepository.UpdateAsync(pago);
                    await _pagoRepository.SaveAsync();

                    _logger.LogInformation("Pago {IdPagoPlan} marcado como RECHAZADO (orden {State})", pago.IdPagoPlan, data.State);
                }
            }
        }

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
