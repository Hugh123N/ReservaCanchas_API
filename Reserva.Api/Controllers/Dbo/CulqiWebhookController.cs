using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Services.Culqi;
using Reserva.Domain.Services.Culqi.Webhook;
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
        private readonly IRepository<Entity.PagoPlan> _pagoPlanRepository;
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly ICulqiService _culqiService;
        private readonly INotificacionService _notificacionService;
        private readonly ILogger<CulqiWebhookController> _logger;

        public CulqiWebhookController(
            IRepository<Entity.PagoPlan> pagoPlanRepository,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.AspNetUsers> userRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
            INotificacionService notificacionService,
            ILogger<CulqiWebhookController> logger)
        {
            _pagoPlanRepository = pagoPlanRepository;
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

                //var signature = Request.Headers["X-Culqi-Signature"].FirstOrDefault();
                //if (!string.IsNullOrEmpty(signature))
                //{
                //    if (!_culqiService.ValidateWebhookSignature(webhookBody, signature))
                //    {
                //        _logger.LogWarning("Firma del webhook inválida");
                //        return Unauthorized(new { message = "Firma inválida" });
                //    }
                //}

                var webhookEvent = JsonSerializer.Deserialize<CulqiWebhookEvent>(webhookBody);
                if (webhookEvent == null)
                {
                    _logger.LogError("Error al deserializar el webhook");
                    return BadRequest(new { message = "Formato de webhook inválido" });
                }

                _logger.LogInformation("Evento de Culqi recibido - Tipo: {Type}, ID: {Id}",
                    webhookEvent.Type, webhookEvent.Id);

                await ProcessWebhookEvent(webhookEvent);

                return Ok(new { message = "Webhook procesado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar webhook de Culqi:");
                return StatusCode(StatusCodes.Status500InternalServerError,
                new {message = "Error interno al procesar webhook"});
            }
        }

        private async Task ProcessWebhookEvent(CulqiWebhookEvent webhookEvent)
        {
            var data = webhookEvent.Data;

            switch (webhookEvent.Type)
            {
                case "charge.creation.succeeded":
                    await HandleChargeSucceeded(data);
                    break;
                case "charge.creation.failed":
                    await HandleChargeFailed(data);
                    break;
                case "subscription.creation.succeeded":
                case "subscription.cancel.succeeded":
                case "subscription.cancel.failed":
                    await HandleSubscriptionEvent(data, webhookEvent.Type);
                    break;
                default:
                    _logger.LogInformation("Evento de Culqi no manejado: {Type}", webhookEvent.Type);
                    break;  
            }
        }

        private async Task HandleChargeSucceeded(string data)
        {// proceso yape y suscription, falta renovacion automatica.
            var charge = JsonSerializer.Deserialize<CulqiChargeWebhookDto>(data);

            _logger.LogInformation("Procesando pago exitoso - ChargeId: {Id}", charge.Id);

            int? proveedorId = null;

            if (charge.Metadata != null && charge.Metadata.TryGetValue("proveedor_id", out var proveedorIdStr) &&
                int.TryParse(proveedorIdStr, out var parsedProveedorId))
            {
                proveedorId = parsedProveedorId;
            }

            var idCustomer = charge.Source.CustomerId;

            var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                p => proveedorId.HasValue ? p.IdProveedor == proveedorId.Value
                    : p.CulqiCustomerId == idCustomer,
                p => p.IdUsuarioNavigation
            );

            await HandlePlanPaymentSucceeded(proveedor, charge.Id, charge.ReferenceCode!, null);
            return;
        }

        private async Task HandlePlanPaymentSucceeded(Entity.Proveedor? proveedor, string charId, string referenceCode, long? nextBillingDate)
        {
            _logger.LogInformation("Procesando pago de plan exitoso - ProveedorPlanId: {Id}, ChargeId: {ChargeId}", proveedor.IdProveedor, charId);

            var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_PAGO.Pagado);

            var proveedorPlans = await _proveedorPlanRepository.FindByAsync(
                pp => pp.IdProveedor == proveedor.IdProveedor 
                    && pp.Activo 
                    && (pp.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE || pp.Estado == Constants.ESTADO_PROV_PLAN.PENDING),
                pp => pp.IdPlanTarifaNavigation
            );
            
            // Obtener el plan más reciente (último en ser creado)
            var proveedorPlan = proveedorPlans?
                .OrderByDescending(x => x.IdProveedorPlan)
                .FirstOrDefault();

            var pagoPlan = new Entity.PagoPlan
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                Monto = proveedorPlan.IdPlanTarifaNavigation?.Precio ?? 0,
                Moneda = Constants.CURRENCY.PEN,
                IdMetodoPago = 1, // Tarjeta
                IdEstadoPago = estadoPagado?.IdEstadoPago ?? 1,
                CulqiChargeId = charId, 
                CodigoOperacion = referenceCode,
                FechaPago = DateTimeOffset.UtcNow,
                Activo = true
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.ACTIVE;
            proveedorPlan.EsActual = true;

            // Actualizar FechaFin y FechaProximoCobro si Culqi provee next_billing_date y es autonew
            if (nextBillingDate.HasValue)
            {
                var nuevoFin = DateTimeOffset.FromUnixTimeSeconds(nextBillingDate.Value);
                proveedorPlan.FechaFin = nuevoFin;
                proveedorPlan.FechaProximoCobro = nuevoFin;
            }
            else
            {
                proveedorPlan.FechaProximoCobro = proveedorPlan.FechaFin;
            }

            // ═══ CANCELACIÓN DIFERIDA DE SUSCRIPCIÓN ANTERIOR ═══
            // Si hay una suscripción anterior pendiente de cancelar (cambio de plan),
            // cancelarla ahora que el nuevo pago fue exitoso.
            if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionIdAnterior))
            {
                try
                {
                    _logger.LogInformation("Cancelando suscripción anterior diferida: {OldSubscriptionId} para ProveedorPlan {Id}",
                        proveedorPlan.CulqiSubscriptionIdAnterior, proveedorPlan.IdProveedorPlan);

                    await _culqiService.CancelSubscriptionAsync(proveedorPlan.CulqiSubscriptionIdAnterior);

                    // Buscar y marcar el ProveedorPlan anterior como CANCELLED
                    var oldPlan = await _proveedorPlanRepository.GetByAsync(
                        pp => pp.CulqiSubscriptionId == proveedorPlan.CulqiSubscriptionIdAnterior
                            && pp.Activo
                    );

                    if (oldPlan != null)
                    {
                        oldPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                        oldPlan.EsActual = false;
                        oldPlan.CancelAtPeriodEnd = false;
                        oldPlan.FechaCancelacion = DateTimeOffset.UtcNow;
                        oldPlan.MotivoCancelacion = "Cancelado por cambio a plan " + proveedorPlan.IdPlane;
                        await _proveedorPlanRepository.UpdateAsync(oldPlan);
                    }

                    // Limpiar la referencia
                    proveedorPlan.CulqiSubscriptionIdAnterior = null;
                    _logger.LogInformation("Suscripción anterior cancelada exitosamente después de confirmar nuevo pago");
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the whole process
                    // The old subscription will need manual cleanup
                    _logger.LogError(ex, "Error al cancelar suscripción anterior diferida. Se requiere limpieza manual.");
                }
            }

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

            _logger.LogInformation("Pago registrado y ProveedorPlan {Id} activado. PagoPlanId: {PagoPlanId}",
                proveedorPlan.IdProveedorPlan, pagoPlan.IdPagoPlan);
        }

        private async Task HandleChargeFailed(string dataEvent)
        {
            var data = JsonSerializer.Deserialize<CulqiChargeFailedWebhookDto>(dataEvent);

            _logger.LogInformation("Procesando pago fallido - ChargeId: {ChargeId}, Code: {Code}, ActionCode: {ActionCode}",
                data.ChargeId, data.Code, data.ActionCode);

            // Intentar obtener información del charge desde Culqi
            Entity.Proveedor? proveedor = null;
            Entity.ProveedorPlan? proveedorPlan = null;

            try
            {
                var charge = await _culqiService.GetChargeAsync(data.ChargeId);
                if (charge != null && !string.IsNullOrEmpty(charge.Email))
                {
                    proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                        x => x.IdUsuarioNavigation != null && x.IdUsuarioNavigation.Email == charge.Email,
                        x => x.IdUsuarioNavigation
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener información del charge {ChargeId} desde Culqi", data.ChargeId);
            }

            // Buscar ProveedorPlan activo del proveedor
            if (proveedor != null)
            {
                var proveedorPlans = await _proveedorPlanRepository.FindByAsync(
                    pp => pp.IdProveedor == proveedor.IdProveedor
                        && pp.Activo
                        && (pp.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE
                            || pp.Estado == Constants.ESTADO_PROV_PLAN.PENDING
                            || pp.Estado == Constants.ESTADO_PROV_PLAN.GRACE),
                    pp => pp.IdPlanTarifaNavigation,
                    pp => pp.IdPlaneNavigation
                );
                
                // Obtener el plan más reciente (último en ser creado)
                proveedorPlan = proveedorPlans?
                    .OrderByDescending(x => x.IdProveedorPlan)
                    .FirstOrDefault();
            }

            if (proveedorPlan == null)
            {
                _logger.LogWarning("ProveedorPlan no encontrado para charge fallido {ChargeId}. ChargeId podría no estar asociado a una suscripción activa.", data.ChargeId);
                return;
            }

            await HandlePlanPaymentFailed(proveedorPlan, proveedor, data.ChargeId, data.Code, data.MerchantMessage);
        }

        private async Task HandlePlanPaymentFailed(
            ProveedorPlan proveedorPlan,
            Entity.Proveedor? proveedor,
            string? chargeId,
            string? errorCode,
            string? merchantMessage)
        {
            _logger.LogInformation("Procesando pago de plan fallido - ProveedorPlanId: {Id}, Estado: {Estado}, ChargeId: {ChargeId}",
                proveedorPlan.IdProveedorPlan, proveedorPlan.Estado, chargeId);

            // Crear PagoPlan RECHAZADO (solo si hay chargeId válido)
            if (!string.IsNullOrEmpty(chargeId))
            {
                var estadoRechazado = await _estadoPagoRepository.GetByAsNoTrackingAsync(
                    e => e.Codigo == Constants.ESTADO_PAGO.Rechazado
                );

                var pagoPlan = new Entity.PagoPlan
                {
                    IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                    Monto = proveedorPlan.IdPlanTarifaNavigation?.Precio ?? 0,
                    Moneda = Constants.CURRENCY.PEN,
                    IdMetodoPago = 1,
                    IdEstadoPago = estadoRechazado?.IdEstadoPago ?? 5,
                    CulqiChargeId = chargeId,
                    CodigoOperacion = errorCode,
                    FechaPago = DateTimeOffset.UtcNow,
                    Activo = true
                };

                await _pagoPlanRepository.AddAsync(pagoPlan);
                await _pagoPlanRepository.SaveAsync();
            }

            // ═══ DIFERENCIAR POR ESTADO ACTUAL ═══
            switch (proveedorPlan.Estado)
            {
                case var estado when estado == Constants.ESTADO_PROV_PLAN.PENDING:
                    // ═══ PRIMERA VEZ o CAMBIO DE PLAN - Pago inicial falló ═══
                    _logger.LogInformation("Pago inicial fallido para ProveedorPlan {Id}. Estado: PENDING → CANCELLED", proveedorPlan.IdProveedorPlan);
                    
                    proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                    proveedorPlan.EsActual = false;
                    proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
                    proveedorPlan.MotivoCancelacion = $"Pago inicial rechazado: {merchantMessage ?? errorCode}";

                    // Si hay suscripción anterior pendiente de cancelar, limpiar la referencia
                    // (el plan anterior sigue activo ya que no se canceló)
                    if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionIdAnterior))
                    {
                        _logger.LogInformation("Cambio de plan fallido. Plan anterior sigue activo. Limpiando referencia.");
                        proveedorPlan.CulqiSubscriptionIdAnterior = null;
                    }
                    break;

                case var estado when estado == Constants.ESTADO_PROV_PLAN.ACTIVE:
                    // ═══ VERIFICAR SI ES CAMBIO DE PLAN o RENOVACIÓN ═══
                    if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionIdAnterior))
                    {
                        // Es un plan NUEVO de cambio de plan → CANCELLED (no GRACE)
                        _logger.LogInformation("Cambio de plan fallido para ProveedorPlan {Id}. Estado: ACTIVE → CANCELLED (plan anterior mantiene servicio)", proveedorPlan.IdProveedorPlan);
                        
                        proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                        proveedorPlan.EsActual = false;
                        proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
                        proveedorPlan.MotivoCancelacion = $"Cambio de plan fallido: {merchantMessage ?? errorCode}";
                        proveedorPlan.CulqiSubscriptionIdAnterior = null;
                    }
                    else
                    {
                        // Es RENOVACIÓN normal → GRACE
                        _logger.LogInformation("Renovación fallida para ProveedorPlan {Id}. Estado: ACTIVE → GRACE", proveedorPlan.IdProveedorPlan);
                        
                        proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.GRACE;
                        proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);
                    }
                    break;

                case var estado when estado == Constants.ESTADO_PROV_PLAN.GRACE:
                    // ═══ REINTENTO en gracia falló ═══
                    _logger.LogInformation("Reintento fallido para ProveedorPlan {Id}. Manteniendo GRACE", proveedorPlan.IdProveedorPlan);
                    
                    proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);
                    break;
            }

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            // Notificar fallo
            var emailFallo = proveedor?.IdUsuarioNavigation?.Email;
            if (!string.IsNullOrEmpty(emailFallo))
            {
                await _notificacionService.NotificarFalloPagoPlanAsync(
                    proveedorPlan,
                    proveedorPlan.IdPlaneNavigation!,
                    emailFallo
                );
            }

            _logger.LogInformation("Pago RECHAZADO registrado y ProveedorPlan {Id} actualizado. Estado: {Estado}",
                proveedorPlan.IdProveedorPlan, proveedorPlan.Estado);
        }

        private async Task HandleSubscriptionEvent(string dataEvento, string eventType)
        {
            var response = JsonSerializer.Deserialize<CulqiSuscriptionWebhookDto>(dataEvento);
            var data = response.Message.Object;

            _logger.LogInformation("Procesando evento de suscripción: {EventType} - SubscriptionId: {SubscriptionId}",
                eventType, data.SubsId);

            // Buscar por CulqiSubscriptionId principal
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.SubsId,
                pp => pp.IdPlaneNavigation,
                pp => pp.PagoPlan
            );

            // Si no se encuentra, buscar en CulqiSubscriptionIdAnterior (cancelación diferida)
            bool esCancelacionDiferida = false;
            if (proveedorPlan == null)
            {
                proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                    pp => pp.CulqiSubscriptionIdAnterior == data.SubsId,
                    pp => pp.IdPlaneNavigation,
                    pp => pp.PagoPlan
                );
                if (proveedorPlan != null)
                {
                    esCancelacionDiferida = true;
                    _logger.LogInformation("Suscripción encontrada como CulqiSubscriptionIdAnterior en ProveedorPlan {Id}",
                        proveedorPlan.IdProveedorPlan);
                }
            }

            if (proveedorPlan == null)
            {
                _logger.LogWarning("ProveedorPlan no encontrado para SubscriptionId: {SubscriptionId}", data.SubsId);
                return;
            }

            var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                p => p.IdProveedor == proveedorPlan.IdProveedor
            );

            switch (eventType)
            {
                case "subscription.created.succeeded":
                    _logger.LogInformation("Suscripción creada para ProveedorPlan {Id}", proveedorPlan.IdProveedorPlan);
                    break;

                case "subscription.cancel.succeeded":
                    // ═══ CANCELACIÓN DIFERIDA ═══
                    // Si el plan ya está CANCELLED (nuestro internal cancel después de plan change),
                    // NO procesar nuevamente - ya fue manejado en HandlePlanPaymentSucceeded
                    if (proveedorPlan.Estado == Constants.ESTADO_PROV_PLAN.CANCELLED)
                    {
                        _logger.LogInformation("ProveedorPlan {Id} ya está CANCELLED. Saltando procesamiento de subscription.cancel.succeeded (cancelación diferida ya procesada)",
                            proveedorPlan.IdProveedorPlan);
                        break;
                    }

                    // ═══ CANCELACIÓN NORMAL (usuario o sistema) ═══
                    //Usuario cancela manualmente de front: El plan permanece ACTIVE hasta FechaFin, pero con renovación cancelada
                    if (proveedorPlan.CancelAtPeriodEnd)
                    { 
                        proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.ACTIVE;
                        proveedorPlan.EsActual = true;
                    }
                    else
                    { 
                        proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                        proveedorPlan.EsActual = false;
                    }
                    proveedorPlan.AutoRenovacion = false;
                    proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;

                    // Si es cancelación de la suscripción anterior (diferida), limpiar referencia
                    if (esCancelacionDiferida && !string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionIdAnterior))
                    {
                        proveedorPlan.CulqiSubscriptionIdAnterior = null;
                        _logger.LogInformation("Referencia CulqiSubscriptionIdAnterior limpiada para ProveedorPlan {Id}",
                            proveedorPlan.IdProveedorPlan);
                    }

                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();

                    _logger.LogInformation("Suscripción cancelada en Culqi para ProveedorPlan {Id}. Plan permanece activo hasta {FechaFin}", 
                        proveedorPlan.IdProveedorPlan, proveedorPlan.FechaFin);
                    break;

                case "subscription.cancel.failed":
                    // revertir los cambios hecos en Cancel autorenew o cancel para cambio de plan.
                    proveedorPlan.CancelAtPeriodEnd = false;

                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();
                    _logger.LogInformation("Suscripción no se pudo cancelar en Culqi para ProveedorPlan {Id}.",
                        proveedorPlan.IdProveedorPlan);
                    break;
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
