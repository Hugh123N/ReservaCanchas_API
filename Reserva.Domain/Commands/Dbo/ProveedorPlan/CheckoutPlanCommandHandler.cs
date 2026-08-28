using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Utils;
using static Reserva.Common.Constants;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CheckoutPlanCommandHandler : CommandHandlerBase<CheckoutPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.Plane> _planeRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly ICulqiService _culqiService;
        private readonly ILogger<CheckoutPlanCommandHandler> _logger;

        public CheckoutPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CheckoutPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.Plane> planeRepository,
            IRepository<Entity.PlanTarifa> tarifaRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
            ILogger<CheckoutPlanCommandHandler> logger
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _planeRepository = planeRepository;
            _tarifaRepository = tarifaRepository;
            _proveedorRepository = proveedorRepository;
            _culqiService = culqiService;
            _logger = logger;
        }

        public override async Task<ResponseDto> HandleCommand(CheckoutPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var dto = request.CheckoutDto;

            var tarifa = await _tarifaRepository.GetByAsync(x => x.IdPlanTarifa == dto.IdPlanTarifa, x => x.IdPlaneNavigation);
            if (tarifa == null)
            {
                response.AddErrorResult("Tarifa no encontrada");
                return response;
            }

            var proveedor = await _proveedorRepository.GetByAsync(x => x.IdProveedor == dto.IdProveedor, x => x.IdUsuarioNavigation);
            if (proveedor == null)
            {
                response.AddErrorResult("Proveedor no encontrado");
                return response;
            }

            var esPagoConTarjeta = dto.PaymentType == "card";
            var esPagoUnico = tarifa.Codigo?.ToUpper() is PLAN_TARIFA.UNIQUE or PLAN_TARIFA.BLACKFRIDAY;
            
            decimal monto = tarifa.Precio;
            if (tarifa.PorcentajeDescuento.HasValue && tarifa.PorcentajeDescuento > 0)
            {
                monto = monto - (monto * tarifa.PorcentajeDescuento.Value / 100);
            }

            // Configuración del plan Culqi según código de tarifa
            var (culqiInterval, culqiIntervalCount, shouldCreateCulqiPlan) = GetCulqiPlanConfig(tarifa);
            var culqiPlanId = $"plan_{tarifa.IdPlanTarifa}";

            // Variables para tracking
            string? customerId = null;
            string? culqiSubscriptionId = null;
            string? culqiChargeId = null;

            if (esPagoUnico)
            {
                // ═══════════════════════════════════════════════════════════════
                // PAGO ÚNICO (UNIQUE/BLACKFRIDAY): SIEMPRE crear Charge
                // NO crear Customer, NO Card, NO Subscription
                // ═══════════════════════════════════════════════════════════════
                _logger.LogInformation("Procesando pago único (Plan {Codigo}) para Proveedor {IdProveedor}", tarifa.Codigo, dto.IdProveedor);

                if (string.IsNullOrEmpty(dto.CulqiToken))
                {
                    response.AddErrorResult("Token de pago requerido");
                    return response;
                }

                try
                {
                    var chargeResponse = await _culqiService.CreateChargeAsync(new CulqiCreateChargeRequest
                    {
                        Amount = CulqiService.ConvertToCents(monto),
                        CurrencyCode = Constants.CURRENCY.PEN,
                        Email = dto.Email,
                        SourceId = dto.CulqiToken,
                        Description = $"Pago plan {tarifa.IdPlaneNavigation?.Nombre} - {tarifa.Nombre}",
                        Metadata = new Dictionary<string, string>
                        {
                            { "proveedor_id", dto.IdProveedor.ToString() },
                            { "plan_id", dto.IdPlane.ToString() },
                            { "tarifa_id", dto.IdPlanTarifa.ToString() },
                            { "tipo", "pago_unico" }
                        }
                    });

                    culqiChargeId = chargeResponse.Id;
                    _logger.LogInformation("Cargo único creado exitosamente: {ChargeId}", culqiChargeId);
                }
                catch (CulqiException ex)
                {
                    response.AddErrorResult(ex.UserMessage ?? "Error al procesar el pago");
                    return response;
                }
            }
            else
            {
                // ═══════════════════════════════════════════════════════════════
                // PLAN DE SUSCRIPCIÓN (MONTHLY/YEARLY)
                // ├─ Si PaymentType == 'card' → Customer + Card + Subscription
                // └─ Si PaymentType == 'order' (Yape) → Charge + Customer, SIN Subscription
                //    (Suscripciones requieren tarjeta para cobros recurrentes)
                // ═══════════════════════════════════════════════════════════════
                
                _logger.LogInformation("Procesando suscripción (Plan {Codigo}, Método: {PaymentType}) para Proveedor {IdProveedor}",
                    tarifa.Codigo, dto.PaymentType, dto.IdProveedor);

                // Paso 1: Crear o obtener Customer en Culqi (requerido para ambos casos)
                customerId = proveedor.CulqiCustomerId;
                if (string.IsNullOrEmpty(customerId))
                {
                    try
                    {
                        var customerRequest = new CulqiCreateCustomerRequest
                        {
                            Email = dto.Email,
                            Code = $"prov_{proveedor.IdProveedor}",
                            FirstName = proveedor.IdUsuarioNavigation?.FirstName,
                            LastName = proveedor.IdUsuarioNavigation?.LastName,
                            Address = "de su casa",
                            AddressCity = "Peru - Provincia",
                            CountryCode = "PE",
                            PhoneNumber = proveedor.Telefono,
                            Metadata = new Dictionary<string, string>
                            {
                                { "proveedor_id", proveedor.IdProveedor.ToString() }
                            }
                        };

                        var customerResponse = await _culqiService.CreateCustomerAsync(customerRequest);
                        customerId = customerResponse.Id;

                        // Guardar CustomerId en el proveedor
                        proveedor.CulqiCustomerId = customerId;
                        await _proveedorRepository.UpdateAsync(proveedor);
                        await _proveedorRepository.SaveAsync();
                    }
                    catch (CulqiException ex)
                    {
                        response.AddErrorResult(ex.UserMessage ?? "Error al crear cliente en Culqi");
                        return response;
                    }
                }

                // Paso 1.5: Si es pago con tarjeta, actualizar tarjeta y cancelar suscripción anterior
                var oldSubscriptionId = (string?)null;
                string? newCardId = null;
                if (esPagoConTarjeta && !string.IsNullOrEmpty(dto.CulqiToken) && !string.IsNullOrEmpty(customerId))
                {
                    try
                    {
                        // Buscar suscripción anterior del proveedor
                        var oldProveedorPlan = await _proveedorPlanRepository.GetByAsync(
                            x => x.IdProveedor == dto.IdProveedor
                                && x.Activo
                                && x.Estado != Constants.ESTADO_PROV_PLAN.CANCELLED
                                && !string.IsNullOrEmpty(x.CulqiSubscriptionId),
                            x => x.IdPlaneNavigation
                        );

                        if (oldProveedorPlan != null)
                        {
                            oldSubscriptionId = oldProveedorPlan.CulqiSubscriptionId;
                            _logger.LogInformation("Suscripción anterior encontrada: {SubscriptionId}", oldSubscriptionId);
                        }

                        // Obtener tarjeta actual del customer
                        var existingCard = await _culqiService.GetCardAsync(customerId);

                        // Crear nueva tarjeta con el token
                        var newCard = await _culqiService.CreateCardAsync(customerId, dto.CulqiToken);
                        newCardId = newCard.Id;

                        // Eliminar tarjeta anterior si existía
                        if (existingCard != null)
                        {
                            await _culqiService.DeleteCardAsync(existingCard.Id);
                            _logger.LogInformation("Tarjeta anterior eliminada: {CardId}", existingCard.Id);
                        }

                        // Cancelar suscripción anterior si existe
                        if (!string.IsNullOrEmpty(oldSubscriptionId))
                        {
                            await _culqiService.CancelSubscriptionAsync(oldSubscriptionId);
                            _logger.LogInformation("Suscripción anterior cancelada: {SubscriptionId}", oldSubscriptionId);

                            // Marcar el ProveedorPlan anterior como inactivo
                            if (oldProveedorPlan != null)
                            {
                                oldProveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                                oldProveedorPlan.EsActual = false;
                                await _proveedorPlanRepository.UpdateAsync(oldProveedorPlan);
                            }
                        }
                    }
                    catch (CulqiException ex)
                    {
                        _logger.LogError(ex, "Error al actualizar tarjeta o cancelar suscripción anterior");
                        response.AddErrorResult(ex.UserMessage ?? "Error al actualizar método de pago");
                        return response;
                    }
                }

                // Paso 2: Crear Plan en Culqi (solo si es suscripción con tarjeta)
                if (esPagoConTarjeta && shouldCreateCulqiPlan)
                {
                    try
                    {
                        var existingPlan = await _culqiService.GetPlanAsync(culqiPlanId);
                        if (existingPlan == null)
                        {
                            var planRequest = new CulqiCreatePlanRequest
                            {
                                Id = culqiPlanId,
                                Name = $"{tarifa.IdPlaneNavigation?.Nombre} - {tarifa.Nombre}",
                                Amount = CulqiService.ConvertToCents(monto),
                                CurrencyCode = Constants.CURRENCY.PEN,
                                Interval = culqiInterval,
                                IntervalCount = culqiIntervalCount,
                                Description = tarifa.Nombre,
                                Metadata = new Dictionary<string, string>
                                {
                                    { "tarifa_id", tarifa.IdPlanTarifa.ToString() },
                                    { "plan_id", tarifa.IdPlane.ToString() }
                                }
                            };

                            await _culqiService.CreatePlanAsync(planRequest);
                        }
                    }
                    catch (CulqiException ex)
                    {
                        _logger.LogWarning("Error al crear plan en Culqi (puede que ya exista): {Message}", ex.Message);
                    }
                }

                // Paso 3: Crear Suscripción o Cargo según método de pago
                CulqiSubscriptionResponse? culqiResponse = null;
                if (esPagoConTarjeta)
                {
                    // ═══════════════════════════════════════════════════════════════
                    // TARJETA: Crear Customer + Card + Subscription
                    // ═══════════════════════════════════════════════════════════════
                    try
                    {
                        var cardIdForSubscription = newCardId ?? dto.CulqiToken;

                        var subscriptionRequest = new CulqiCreateSubscriptionRequest
                        {
                            PlanId = culqiPlanId,
                            CustomerId = customerId!,
                            CardId = cardIdForSubscription,
                            Metadata = new Dictionary<string, string>
                            {
                                { "plan_id", dto.IdPlane.ToString() },
                                { "proveedor_id", dto.IdProveedor.ToString() },
                                { "tarifa_id", dto.IdPlanTarifa.ToString() },
                                { "tipo", "plan_proveedor" }
                            }
                        };

                        culqiResponse = await _culqiService.CreateSubscriptionAsync(subscriptionRequest);
                        culqiSubscriptionId = culqiResponse.Id;
                    }
                    catch (CulqiException ex)
                    {
                        response.AddErrorResult(ex.UserMessage ?? "Error al procesar la suscripción con Culqi");
                        return response;
                    }
                }
                else
                {
                    // ═══════════════════════════════════════════════════════════════
                    // YAPE/ORDER en plan de suscripción: Solo crear Charge
                    // NO crear Subscription (requiere tarjeta para cobros recurrentes)
                    // El usuario deberá agregar tarjeta después para activar renovación
                    // ═══════════════════════════════════════════════════════════════
                    try
                    {
                        var chargeResponse = await _culqiService.CreateChargeAsync(new CulqiCreateChargeRequest
                        {
                            Amount = CulqiService.ConvertToCents(monto),
                            CurrencyCode = Constants.CURRENCY.PEN,
                            Email = dto.Email,
                            SourceId = dto.CulqiToken,
                            Description = $"Pago inicial plan {tarifa.IdPlaneNavigation?.Nombre} - {tarifa.Nombre}",
                            Metadata = new Dictionary<string, string>
                            {
                                { "proveedor_id", dto.IdProveedor.ToString() },
                                { "plan_id", dto.IdPlane.ToString() },
                                { "tarifa_id", dto.IdPlanTarifa.ToString() },
                                { "tipo", "pago_inicial_yape" }
                            }
                        });

                        culqiChargeId = chargeResponse.Id;
                        _logger.LogInformation("Cargo Yape creado para plan de suscripción: {ChargeId}", culqiChargeId);
                    }
                    catch (CulqiException ex)
                    {
                        response.AddErrorResult(ex.UserMessage ?? "Error al procesar el pago con Yape");
                        return response;
                    }
                }
            }

            // Calcular fechas basadas en la duración de la tarifa
            var fechaInicio = DateTimeOffset.UtcNow;
            var billingDay = fechaInicio.Day;
            var fechaFin = DateTimeHelper.GetNextBillingDate(fechaInicio, billingDay, tarifa.DuracionDias ?? 0);
            
            // Determinar si es suscripción con tarjeta (para fechas y estados)
            var esSuscripcionConTarjeta = !esPagoUnico && dto.PaymentType == "card";
            
            // FechaProximoCobro solo se setea para suscripciones con tarjeta (webhook la actualizará)
            var fechaProximoCobro = esSuscripcionConTarjeta && tarifa.PermiteAutoRenovacion == true
                ? fechaFin : (DateTimeOffset?)null;

            var proveedorPlan = new Entity.ProveedorPlan
            {
                IdProveedor = dto.IdProveedor,
                IdPlane = dto.IdPlane,
                IdPlanTarifa = dto.IdPlanTarifa,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                FechaProximoCobro = fechaProximoCobro,
                // Estado: ACTIVE para pagos directos (único o Yape), PENDING solo para suscripción con tarjeta
                Estado = esSuscripcionConTarjeta ? Constants.ESTADO_PROV_PLAN.PENDING : Constants.ESTADO_PROV_PLAN.ACTIVE,
                // AutoRenovacion: true solo si hay tarjeta y plan lo permite
                AutoRenovacion = esSuscripcionConTarjeta && (tarifa.PermiteAutoRenovacion ?? false),
                EsActual = true,
                CulqiSubscriptionId = culqiSubscriptionId, // null para pagos únicos y Yape en suscripción
                CulqiCustomerId = customerId,
                GracePeriodHasta = null
            };

            var pagosAnteriores = await _proveedorPlanRepository.FindByAsync(x => x.IdProveedor == dto.IdProveedor && x.EsActual && x.Activo);
            foreach (var pp in pagosAnteriores)
            {
                pp.EsActual = false;
            }

            await _proveedorPlanRepository.UpdateAsync(pagosAnteriores.ToArray());

            await _proveedorPlanRepository.AddAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            string mensajeExito;
            if (esPagoUnico)
            {
                // Plan UNIQUE/BLACKFRIDAY: siempre activo directamente
                mensajeExito = "Pago registrado. Tu plan está activo.";
            }
            else if (esSuscripcionConTarjeta)
            {
                // Suscripción con tarjeta: esperando webhook
                mensajeExito = "Suscripción iniciada. Espera la confirmación del webhook de Culqi.";
            }
            else
            {
                // Yape en plan de suscripción: activo pero sin renovación automática
                mensajeExito = "Pago registrado. Tu plan está activo. Para activar la renovación automática, agrega una tarjeta desde tu perfil.";
            }
            
            response.AddOkResult(mensajeExito);
            return response;
        }

        /// <summary>
        /// Obtiene la configuración del plan Culqi según el código de la tarifa
        /// </summary>
        private (string interval, int intervalCount, bool shouldCreateCulqiPlan) GetCulqiPlanConfig(Entity.PlanTarifa tarifa)
        {
            return tarifa.Codigo?.ToUpper() switch
            {
                PLAN_TARIFA.MONTHLY => ("months", 1, true),
                PLAN_TARIFA.YEARLY => ("years", 1, true),
                _ => ("months", 1, false)  // BLACKFRIDAY, UNIQUE, etc. - No crear plan Culqi
            };
        }
    }

    public class CheckoutPlanCommandValidator : CommandValidatorBase<CheckoutPlanCommand>
    {
        public CheckoutPlanCommandValidator()
        {
            RequiredInformation(x => x.CheckoutDto).DependentRules(() =>
            {
                RuleFor(x => x.CheckoutDto.IdProveedor)
                    .GreaterThan(0)
                    .WithMessage("El proveedor es requerido");

                RuleFor(x => x.CheckoutDto.IdPlane)
                    .GreaterThan(0)
                    .WithMessage("El plan es requerido");

                RuleFor(x => x.CheckoutDto.IdPlanTarifa)
                    .GreaterThan(0)
                    .WithMessage("La tarifa es requerida");

                // CulqiToken es opcional (para suscripciones, el cliente ya puede tener una tarjeta guardada)
                RuleFor(x => x.CheckoutDto.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage("El email es requerido");
            });
        }
    }
}