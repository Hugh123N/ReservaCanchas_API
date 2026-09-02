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

                case "order.status.changed":
                    await HandleOrderStatusChanged(data);
                    break;

                case "subscription.creation.succeeded":
                case "subscription.update.succeeded":
                case "subscription.cancel.succeeded":
                case "subscription.charge.succeeded":
                case "subscription.charge.failed":
                    
                    await HandleSubscriptionEvent(data, webhookEvent.Type);
                    break;

                default:
                    _logger.LogInformation("Evento de Culqi no manejado: {Type}", webhookEvent.Type);
                    break;  
            }
        }

        private async Task HandleChargeSucceeded(string data)
        {
            var charge = JsonSerializer.Deserialize<CulqiChargeWebhookDto>(data);

            _logger.LogInformation("Procesando pago exitoso - ChargeId: {Id}", charge.Id);

            var proveedorId = int.Parse(charge.Metadata?["proveedor_id"]);

            var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                p => p.IdProveedor == proveedorId,
                p => p.IdUsuarioNavigation
            );

            await HandlePlanPaymentSucceeded(proveedor, proveedorId, charge.Id, charge.ReferenceCode!, null);
            return;
        }

        private async Task<ProveedorPlan?> FindProveedorPlanForCharge(int? idProveedor)
        {
            // 1. Buscar por metadata proveedor_id (pagos únicos, Yape en plan, renovaciones)
            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.IdProveedor == idProveedor
                    && pp.Activo
                    && (pp.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE
                        || pp.Estado == Constants.ESTADO_PROV_PLAN.PENDING),
                pp => pp.IdPlaneNavigation
            );

            if (proveedorPlan != null)
                return proveedorPlan;

            // 2. Fallback: buscar el plan activo más reciente del proveedor
            if (idProveedor != null)
            {
                return await _proveedorPlanRepository.GetByAsync(
                    pp => pp.IdProveedor == idProveedor
                        && pp.EsActual
                        && pp.Activo,
                    pp => pp.IdPlaneNavigation
                );
            }

            return null;
        }

        private async Task HandlePlanPaymentSucceeded(Entity.Proveedor? proveedor, int idProveedor, string charId, string referenceCode, long? nextBillingDate)
        {
            _logger.LogInformation("Procesando pago de plan exitoso - ProveedorPlanId: {Id}, ChargeId: {ChargeId}", idProveedor, charId);

            var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(e => e.Codigo == Constants.ESTADO_PAGO.Pagado);

            var pagosAnteriores = await _proveedorPlanRepository.FindByAsync(x => x.IdProveedor == proveedor.IdProveedor && x.EsActual && x.Activo);
            foreach (var pp in pagosAnteriores)
            {
                pp.EsActual = false;
            }
            await _proveedorPlanRepository.UpdateAsync(pagosAnteriores.ToArray());

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.IdProveedor == idProveedor && pp.Activo && (pp.Estado == Constants.ESTADO_PROV_PLAN.PENDING),
                pp => pp.IdPlanTarifaNavigation
            );

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

            // Actualizar FechaFin y FechaProximoCobro si Culqi provee next_billing_date
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
            var charge = JsonSerializer.Deserialize<CulqiChargeWebhookObject>(dataEvent);

            _logger.LogInformation("Procesando pago fallido - ChargeId: {ChargeId}", charge.CharId);

            // Buscar ProveedorPlan: primero por metadata, luego por subscription
            var proveedorPlan = await FindProveedorPlanForCharge(1);

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                await HandlePlanPaymentFailed(proveedorPlan, proveedor, charge);
                return;
            }

            _logger.LogWarning("ProveedorPlan no encontrado para ChargeId fallido: {ChargeId}", charge.CharId);
        }

        private async Task HandlePlanPaymentFailed(ProveedorPlan proveedorPlan, Entity.Proveedor? proveedor, CulqiChargeWebhookObject data)
        {
            _logger.LogInformation("Procesando pago de plan fallido - ProveedorPlanId: {Id}, ChargeId: {ChargeId}",
                proveedorPlan.IdProveedorPlan, data.CharId);

            // Crear PagoPlan RECHAZADO
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
                CulqiChargeId = data.CharId,
                CodigoOperacion = data.ReferenceCode,
                FechaPago = DateTimeOffset.UtcNow,
                Activo = true
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            // Cambiar ProveedorPlan a GRACE
            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.GRACE;
            proveedorPlan.GracePeriodHasta = DateTimeOffset.UtcNow.AddDays(5);

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

            _logger.LogInformation("Pago RECHAZADO registrado y ProveedorPlan {Id} en GRACE. PagoPlanId: {PagoPlanId}",
                proveedorPlan.IdProveedorPlan, pagoPlan.IdPagoPlan);
        }

        private async Task HandleSubscriptionEvent(string dataEvento, string eventType)
        {
            var data = JsonSerializer.Deserialize<CulqiSubscriptionWebhookObject>(dataEvento);

            _logger.LogInformation("Procesando evento de suscripción: {EventType} - SubscriptionId: {SubscriptionId}",
                eventType, data.SubsId);

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                pp => pp.CulqiSubscriptionId == data.SubsId,
                pp => pp.IdPlaneNavigation,
                pp => pp.PagoPlan
            );

            if (proveedorPlan == null)
            {
                _logger.LogWarning("ProveedorPlan no encontrado para SubscriptionId: {SubscriptionId}", data.SubsId);
                return;
            }

            var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                p => p.IdProveedor == proveedorPlan.IdProveedor,
                p => p.IdUsuarioNavigation
            );

            switch (eventType)
            {
                case "subscription.created.succeeded":
                    _logger.LogInformation("Suscripción creada para ProveedorPlan {Id}", proveedorPlan.IdProveedorPlan);

                    //var data = dataEvent.Deserialize<CulqiSubscriptionWebhookData>();

                    
                    break;

                case "subscription.updated.succeeded":
                    
                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();
                    
                    break;

                case "subscription.cancel.succeeded":
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

                case "subscription.charge.succeeded":
                    _logger.LogInformation("Suscripción pagado para ProveedorPlan {Id}", proveedorPlan.IdProveedorPlan);

                    proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.ACTIVE;
                    proveedorPlan.EsActual = true;
                    await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
                    await _proveedorPlanRepository.SaveAsync();
                    break;
            }
        }

        private async Task HandleOrderStatusChanged(string dataEvent)
        {
            var data = JsonSerializer.Deserialize<CulqiWebhookDataTest>(dataEvent);
            var datatest = JsonSerializer.Deserialize<CulqiChargeWebhookObject>(dataEvent);

            _logger.LogInformation("Procesando cambio de estado de orden - OrderId: {OrderId}, Estado: {State}",
                data.Id, data.State);

            // Buscar ProveedorPlan por metadata
            var proveedorPlan = await FindProveedorPlanForCharge(1);

            if (proveedorPlan != null)
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdProveedor == proveedorPlan.IdProveedor,
                    p => p.IdUsuarioNavigation
                );
                if (data.State == "paid" || data.State == "paid_out")
                {
                    await HandlePlanPaymentSucceeded(proveedor, 1, "", "",54346643);
                }
                else if (data.State == "expired" || data.State == "deleted")
                {
                    await HandlePlanPaymentFailed(proveedorPlan, proveedor, datatest);
                }
                return;
            }

            _logger.LogWarning("ProveedorPlan no encontrado para OrderId: {OrderId}", data.Id);
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
