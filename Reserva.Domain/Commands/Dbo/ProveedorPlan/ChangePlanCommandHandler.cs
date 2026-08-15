using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using static Reserva.Common.Constants;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class ChangePlanCommandHandler : CommandHandlerBase<ChangePlanCommand, ChangePlanResponseDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.Plane> _planeRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.MetodoPago> _metodoPagoRepository;
        private readonly IRepository<Entity.PagoPlan> _pagoPlanRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly ICulqiService _culqiService;
        private readonly ILogger<ChangePlanCommandHandler> _logger;

        public ChangePlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ChangePlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.Plane> planeRepository,
            IRepository<Entity.PlanTarifa> tarifaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.MetodoPago> metodoPagoRepository,
            IRepository<Entity.PagoPlan> pagoPlanRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
            ILogger<ChangePlanCommandHandler> logger
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _planeRepository = planeRepository;
            _tarifaRepository = tarifaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _metodoPagoRepository = metodoPagoRepository;
            _pagoPlanRepository = pagoPlanRepository;
            _proveedorRepository = proveedorRepository;
            _culqiService = culqiService;
            _logger = logger;
        }

        public override async Task<ResponseDto<ChangePlanResponseDto>> HandleCommand(ChangePlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ChangePlanResponseDto>();
            var dto = request.ChangePlanDto;

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                x => x.IdProveedorPlan == dto.IdProveedorPlan,
                x => x.IdPlanTarifaNavigation,
                x => x.IdPlaneNavigation
            );
            if (proveedorPlan == null)
            {
                response.AddErrorResult("Suscripción no encontrada");
                return response;
            }

            if (!proveedorPlan.EsActual || proveedorPlan.Estado != Constants.ESTADO_PROV_PLAN.ACTIVE)
            {
                response.AddErrorResult("La suscripción no está activa");
                return response;
            }

            // Determinar si el NUEVO plan es pago único según su código
            // UNIQUE/BLACKFRIDAY = pago único, MONTHLY/YEARLY = suscripción
            var nuevaTarifa = await _tarifaRepository.GetByAsync(x => x.IdPlanTarifa == dto.IdNuevaPlanTarifa, x => x.IdPlaneNavigation);
            if (nuevaTarifa == null)
            {
                response.AddErrorResult("Nueva tarifa no encontrada");
                return response;
            }
            var esPagoUnico = nuevaTarifa.Codigo?.ToUpper() is PLAN_TARIFA.UNIQUE or PLAN_TARIFA.BLACKFRIDAY;
            
            // Para planes de suscripción, validar que el plan actual tenga CulqiSubscriptionId
            if (!esPagoUnico && string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
            {
                response.AddErrorResult("El plan actual no tiene suscripción activa en Culqi");
                return response;
            }

            var tarifaActual = proveedorPlan.IdPlanTarifaNavigation;

            decimal precioActual = tarifaActual.Precio;
            if (tarifaActual.PorcentajeDescuento.HasValue && tarifaActual.PorcentajeDescuento > 0)
                precioActual = precioActual - (precioActual * tarifaActual.PorcentajeDescuento.Value / 100);

            decimal precioNuevo = nuevaTarifa.Precio;
            if (nuevaTarifa.PorcentajeDescuento.HasValue && nuevaTarifa.PorcentajeDescuento > 0)
                precioNuevo = precioNuevo - (nuevaTarifa.PorcentajeDescuento.Value / 100);

            var ahora = DateTimeOffset.UtcNow;
            var diasRestantes = Math.Max(1, (int)(proveedorPlan.FechaFin - ahora).TotalDays);
            var duracionNueva = nuevaTarifa.DuracionDias;

            decimal creditoPlanActual = tarifaActual.DuracionDias > 0
                ? Math.Round((precioActual / tarifaActual.DuracionDias) * diasRestantes, 2)
                : 0;

            decimal cargoPlanNuevo = duracionNueva > 0
                ? Math.Round((precioNuevo / duracionNueva) * diasRestantes, 2)
                : 0;

            decimal saldoAFavorAnterior = proveedorPlan.SaldoFavor;
            decimal montoProrrateo = cargoPlanNuevo - creditoPlanActual - saldoAFavorAnterior;
            decimal nuevoSaldoAFavor = 0;
            bool esUpgrade = cargoPlanNuevo > creditoPlanActual;

            if (montoProrrateo < 0)
            {
                nuevoSaldoAFavor = Math.Abs(montoProrrateo);
                montoProrrateo = 0;
            }

            _logger.LogInformation(
                "Cambio de plan - Plan Actual: {PrecioActual}, Plan Nuevo: {PrecioNuevo}, " +
                "Días restantes: {Dias}, Crédito: {Credito}, Cargo: {Cargo}, Prorrateo: {Prorrateo}, Saldo: {Saldo}",
                precioActual, precioNuevo, diasRestantes, creditoPlanActual, cargoPlanNuevo, montoProrrateo, nuevoSaldoAFavor);

            // Determinar método de pago del usuario
            var esPagoConTarjeta = dto.PaymentType == "card";

            // Para pagos únicos, no se requiere Customer en Culqi
            Entity.Proveedor? proveedor = null;
            if (!esPagoUnico)
            {
                proveedor = await _proveedorRepository.GetByAsync(
                    x => x.IdProveedor == proveedorPlan.IdProveedor,
                    x => x.IdUsuarioNavigation);

                if (proveedor == null || string.IsNullOrEmpty(proveedor.CulqiCustomerId))
                {
                    response.AddErrorResult("Proveedor no tiene customer en Culqi");
                    return response;
                }
            }
            else
            {
                proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(
                    x => x.IdProveedor == proveedorPlan.IdProveedor,
                    x => x.IdUsuarioNavigation);
            }

            // Manejar prorrateo según método de pago
            if (esUpgrade && montoProrrateo > 0)
            {
                if (esPagoUnico || !esPagoConTarjeta)
                {
                    // ═══════════════════════════════════════════════════════════════
                    // PAGO ÚNICO o YAPE en plan de suscripción: Crear Charge
                    // ═══════════════════════════════════════════════════════════════
                    if (string.IsNullOrEmpty(dto.CulqiToken))
                    {
                        response.AddErrorResult("Token de pago requerido");
                        return response;
                    }

                    try
                    {
                        var chargeRequest = new CulqiCreateChargeRequest
                        {
                            Amount = CulqiService.ConvertToCents(montoProrrateo),
                            CurrencyCode = Constants.CURRENCY.PEN,
                            Email = dto.Email,
                            SourceId = dto.CulqiToken,
                            Description = "Prorrateo cambio de plan - Upgrade",
                            Metadata = new Dictionary<string, string>
                            {
                                { "proveedor_plan_id", proveedorPlan.IdProveedorPlan.ToString() },
                                { "tipo", "prorrateo_upgrade" }
                            }
                        };

                        var chargeResponse = await _culqiService.CreateChargeAsync(chargeRequest);
                        _logger.LogInformation("Cargo prorrateo creado en Culqi - ChargeId: {ChargeId}", chargeResponse.Id);

                        var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pagado);
                        var metodoPagoProrrateo = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Yape);

                        var pagoProrrateo = new Entity.PagoPlan
                        {
                            IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                            Monto = montoProrrateo,
                            Moneda = Constants.CURRENCY.PEN,
                            IdMetodoPago = metodoPagoProrrateo?.IdMetodoPago ?? 1,
                            IdEstadoPago = estadoPagado?.IdEstadoPago ?? 1,
                            CulqiChargeId = chargeResponse.Id,
                            CodigoOperacion = chargeResponse.ReferenceCode,
                            FechaPago = DateTimeOffset.UtcNow
                        };

                        await _pagoPlanRepository.AddAsync(pagoProrrateo);
                    }
                    catch (CulqiException ex)
                    {
                        _logger.LogError(ex, "Error al cobrar prorrateo de upgrade");
                        response.AddErrorResult(ex.UserMessage ?? "Error al procesar el cobro del prorrateo");
                        return response;
                    }
                }
                else
                {
                    // ═══════════════════════════════════════════════════════════════
                    // TARJETA en plan de suscripción: Actualizar tarjeta y crear cargo
                    // ═══════════════════════════════════════════════════════════════
                    if (!string.IsNullOrEmpty(dto.CulqiToken))
                    {
                        try
                        {
                            var existingCard = await _culqiService.GetCardAsync(proveedor!.CulqiCustomerId!);
                            var newCard = await _culqiService.CreateCardAsync(proveedor!.CulqiCustomerId!, dto.CulqiToken);

                            if (existingCard != null)
                            {
                                await _culqiService.DeleteCardAsync(existingCard.Id);
                            }

                            _logger.LogInformation("Tarjeta actualizada para cambio de plan");
                        }
                        catch (CulqiException ex)
                        {
                            _logger.LogError(ex, "Error al actualizar tarjeta");
                            response.AddErrorResult(ex.UserMessage ?? "Error al procesar el método de pago");
                            return response;
                        }
                    }

                    try
                    {
                        var chargeRequest = new CulqiCreateChargeRequest
                        {
                            Amount = CulqiService.ConvertToCents(montoProrrateo),
                            CurrencyCode = Constants.CURRENCY.PEN,
                            CustomerId = proveedor!.CulqiCustomerId,
                            Email = dto.Email,
                            Description = "Prorrateo cambio de plan - Upgrade",
                            Metadata = new Dictionary<string, string>
                            {
                                { "proveedor_plan_id", proveedorPlan.IdProveedorPlan.ToString() },
                                { "tipo", "prorrateo_upgrade" }
                            }
                        };

                        var chargeResponse = await _culqiService.CreateChargeAsync(chargeRequest);
                        _logger.LogInformation("Cargo prorrateo creado en Culqi - ChargeId: {ChargeId}", chargeResponse.Id);

                        var estadoPagado = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pagado);
                        var metodoPagoProrrateo = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Tarjeta);

                        var pagoProrrateo = new Entity.PagoPlan
                        {
                            IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                            Monto = montoProrrateo,
                            Moneda = Constants.CURRENCY.PEN,
                            IdMetodoPago = metodoPagoProrrateo?.IdMetodoPago ?? 1,
                            IdEstadoPago = estadoPagado?.IdEstadoPago ?? 1,
                            CulqiChargeId = chargeResponse.Id,
                            CodigoOperacion = chargeResponse.ReferenceCode,
                            FechaPago = DateTimeOffset.UtcNow
                        };

                        await _pagoPlanRepository.AddAsync(pagoProrrateo);
                    }
                    catch (CulqiException ex)
                    {
                        _logger.LogError(ex, "Error al cobrar prorrateo de upgrade");
                        response.AddErrorResult(ex.UserMessage ?? "Error al procesar el cobro del prorrateo");
                        return response;
                    }
                }
            }

            // Cancelar suscripción anterior (solo si el plan anterior era de suscripción)
            if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
            {
                try
                {
                    await _culqiService.CancelSubscriptionAsync(proveedorPlan.CulqiSubscriptionId);
                    _logger.LogInformation("Suscripción anterior cancelada en Culqi: {SubscriptionId}", proveedorPlan.CulqiSubscriptionId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al cancelar suscripción anterior en Culqi (continuando)");
                }
            }

            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
            proveedorPlan.EsActual = false;
            proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
            proveedorPlan.MotivoCancelacion = "Cambio de plan";

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);

            // Configuración del plan Culqi según código de tarifa
            var (culqiInterval, culqiIntervalCount, shouldCreateCulqiPlan) = GetCulqiPlanConfig(nuevaTarifa);
            var nuevoCulqiPlanId = $"plan_{nuevaTarifa.IdPlanTarifa}";

            // Crear plan Culqi solo si es suscripción con tarjeta
            if (esPagoConTarjeta && shouldCreateCulqiPlan)
            {
                try
                {
                    var existingPlan = await _culqiService.GetPlanAsync(nuevoCulqiPlanId);
                    if (existingPlan == null)
                    {
                        var planRequest = new CulqiCreatePlanRequest
                        {
                            Id = nuevoCulqiPlanId,
                            Name = $"{nuevaTarifa.IdPlaneNavigation?.Nombre} - {nuevaTarifa.Nombre}",
                            Amount = CulqiService.ConvertToCents(precioNuevo),
                            CurrencyCode = Constants.CURRENCY.PEN,
                            Interval = culqiInterval,
                            IntervalCount = culqiIntervalCount,
                            Description = nuevaTarifa.Nombre,
                            Metadata = new Dictionary<string, string>
                            {
                                { "tarifa_id", nuevaTarifa.IdPlanTarifa.ToString() },
                                { "plan_id", nuevaTarifa.IdPlane.ToString() }
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

            // Crear nueva suscripción o cargo según tipo de plan y método de pago
            string? nuevaSuscripcionId = null;
            if (esPagoConTarjeta && !esPagoUnico && shouldCreateCulqiPlan)
            {
                // ═══════════════════════════════════════════════════════════════
                // TARJETA en plan de suscripción: Crear Suscripción
                // ═══════════════════════════════════════════════════════════════
                try
                {
                    var nuevaSuscripcion = await _culqiService.CreateSubscriptionAsync(new CulqiCreateSubscriptionRequest
                    {
                        PlanId = nuevoCulqiPlanId,
                        CustomerId = proveedor!.CulqiCustomerId!,
                        Metadata = new Dictionary<string, string>
                        {
                            { "tipo", "plan_change" },
                            { "proveedor_id", proveedorPlan.IdProveedor.ToString() },
                            { "plan_anterior_id", proveedorPlan.IdProveedorPlan.ToString() },
                            { "prorrateo", montoProrrateo.ToString("F2") }
                        }
                    });

                    nuevaSuscripcionId = nuevaSuscripcion.Id;
                    _logger.LogInformation("Nueva suscripción creada en Culqi: {SubscriptionId}", nuevaSuscripcionId);
                }
                catch (CulqiException ex)
                {
                    response.AddErrorResult(ex.UserMessage ?? "Error al crear la nueva suscripción en Culqi");
                    return response;
                }
            }

            var nuevoProveedorPlan = new Entity.ProveedorPlan
            {
                IdProveedor = dto.IdProveedor,
                IdPlane = dto.IdNuevoPlane,
                IdPlanTarifa = dto.IdNuevaPlanTarifa,
                FechaInicio = ahora,
                FechaFin = ahora.AddDays(duracionNueva),
                FechaProximoCobro = esPagoConTarjeta && !esPagoUnico ? ahora.AddDays(duracionNueva) : null,
                // Estado: ACTIVE para pagos directos, PENDING solo para suscripción con tarjeta
                Estado = esPagoConTarjeta && !esPagoUnico ? Constants.ESTADO_PROV_PLAN.PENDING : Constants.ESTADO_PROV_PLAN.ACTIVE,
                // AutoRenovacion: true solo si hay tarjeta y plan lo permite
                AutoRenovacion = esPagoConTarjeta && !esPagoUnico && (nuevaTarifa.PermiteAutoRenovacion ?? false),
                EsActual = true,
                CulqiSubscriptionId = nuevaSuscripcionId, // null para pagos únicos y Yape en suscripción
                CulqiCustomerId = proveedor?.CulqiCustomerId,
                SaldoFavor = nuevoSaldoAFavor,
                UserNameCreate = proveedorPlan.UserNameCreate
            };

            await _proveedorPlanRepository.AddAsync(nuevoProveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            var estadoPendiente = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pendiente);
            var codigoMetodoPago = esPagoUnico || !esPagoConTarjeta ? Constants.METODO_PAGO.Yape : Constants.METODO_PAGO.Tarjeta;
            var metodoPago = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == codigoMetodoPago);

            var pagoPlan = new Entity.PagoPlan
            {
                IdProveedorPlan = nuevoProveedorPlan.IdProveedorPlan,
                Monto = 0,
                Moneda = Constants.CURRENCY.PEN,
                IdMetodoPago = metodoPago?.IdMetodoPago ?? 1,
                IdEstadoPago = estadoPendiente?.IdEstadoPago ?? 1,
                CulqiChargeId = nuevaSuscripcionId,
                CodigoOperacion = null
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            var changePlanResponse = new ChangePlanResponseDto
            {
                IdProveedorPlan = nuevoProveedorPlan.IdProveedorPlan,
                IdNuevoPlane = dto.IdNuevoPlane,
                IdNuevaPlanTarifa = dto.IdNuevaPlanTarifa,
                CulqiSubscriptionId = nuevaSuscripcionId,
                MontoProrrateado = esUpgrade ? montoProrrateo : 0,
                SaldoAFavor = nuevoSaldoAFavor,
                Moneda = Constants.CURRENCY.PEN,
                Estado = nuevoProveedorPlan.Estado,
                NuevaFechaFin = nuevoProveedorPlan.FechaFin,
                NuevaFechaProximoCobro = nuevoProveedorPlan.FechaProximoCobro,
                EsUpgrade = esUpgrade
            };

            response.UpdateData(changePlanResponse);
            
            string mensajeExito;
            if (esPagoUnico)
            {
                // Plan UNIQUE/BLACKFRIDAY: siempre activo directamente
                mensajeExito = esUpgrade
                    ? $"Plan cambiado. Cobro prorrateado: S/ {montoProrrateo:F2}."
                    : $"Plan cambiado. Saldo a favor: S/ {nuevoSaldoAFavor:F2}.";
            }
            else if (esPagoConTarjeta)
            {
                // Suscripción con tarjeta: esperando webhook
                mensajeExito = esUpgrade
                    ? $"Plan cambiado. Cobro prorrateado: S/ {montoProrrateo:F2}. Esperando confirmación del webhook."
                    : $"Plan cambiado. Saldo a favor: S/ {nuevoSaldoAFavor:F2}. Esperando confirmación del webhook.";
            }
            else
            {
                // Yape en plan de suscripción: activo pero sin renovación automática
                mensajeExito = esUpgrade
                    ? $"Plan cambiado. Cobro prorrateado: S/ {montoProrrateo:F2}."
                    : $"Plan cambiado. Saldo a favor: S/ {nuevoSaldoAFavor:F2}.";
                mensajeExito += " Para activar la renovación automática, agrega una tarjeta desde tu perfil.";
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
                "MONTHLY" => ("months", 1, true),
                "YEARLY" => ("years", 1, true),
                _ => ("months", 1, false)  // BLACKFRIDAY, UNIQUE, etc. - No crear plan Culqi
            };
        }
    }

    public class ChangePlanCommandValidator : CommandValidatorBase<ChangePlanCommand>
    {
        public ChangePlanCommandValidator()
        {
            RequiredInformation(x => x.ChangePlanDto).DependentRules(() =>
            {
                RuleFor(x => x.ChangePlanDto.IdProveedorPlan)
                    .GreaterThan(0)
                    .WithMessage("El ID de suscripción es requerido");

                RuleFor(x => x.ChangePlanDto.IdNuevoPlane)
                    .GreaterThan(0)
                    .WithMessage("El nuevo plan es requerido");

                RuleFor(x => x.ChangePlanDto.IdNuevaPlanTarifa)
                    .GreaterThan(0)
                    .WithMessage("La nueva tarifa es requerida");

                RuleFor(x => x.ChangePlanDto.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .When(x => !string.IsNullOrEmpty(x.ChangePlanDto.CulqiToken))
                    .WithMessage("El email es requerido cuando se envía un token de tarjeta");
            });
        }
    }
}
