using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Entity;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CheckoutPlanCommandHandler : CommandHandlerBase<CheckoutPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.Plane> _planeRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.MetodoPago> _metodoPagoRepository;
        private readonly IRepository<Entity.PagoPlan> _pagoPlanRepository;
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
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.MetodoPago> metodoPagoRepository,
            IRepository<Entity.PagoPlan> pagoPlanRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
            ILogger<CheckoutPlanCommandHandler> logger
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

            var estadoPendiente = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pendiente);
            var metodoPago = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Yape);

            decimal monto = tarifa.Precio;
            if (tarifa.PorcentajeDescuento.HasValue && tarifa.PorcentajeDescuento > 0)
            {
                monto = monto - (monto * tarifa.PorcentajeDescuento.Value / 100);
            }

            // Paso 1: Crear o obtener Customer en Culqi
            var customerId = proveedor.CulqiCustomerId;
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

            // Paso 2: Crear Plan en Culqi (si no existe)
            var culqiPlanId = $"plan_{tarifa.IdPlanTarifa}";
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
                        Interval = "months",
                        IntervalCount = tarifa.DuracionDias >= 30 ? tarifa.DuracionDias / 30 : 1,
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

            // Paso 3: Crear Suscripción en Culqi
            CulqiSubscriptionResponse? culqiResponse = null;
            try
            {
                var subscriptionRequest = new CulqiCreateSubscriptionRequest
                {
                    PlanId = culqiPlanId,
                    CustomerId = customerId!,
                    CardId = dto.CulqiToken,
                    Metadata = new Dictionary<string, string>
                    {
                        { "plan_id", dto.IdPlane.ToString() },
                        { "proveedor_id", dto.IdProveedor.ToString() },
                        { "tarifa_id", dto.IdPlanTarifa.ToString() },
                        { "tipo", "plan_proveedor" }
                    }
                };

                culqiResponse = await _culqiService.CreateSubscriptionAsync(subscriptionRequest);
            }
            catch (CulqiException ex)
            {
                response.AddErrorResult(ex.UserMessage ?? "Error al procesar la suscripción con Culqi");
                return response;
            }

            // Calcular fechas basadas en la duración de la tarifa
            var fechaInicio = DateTimeOffset.UtcNow;
            var fechaFin = fechaInicio.AddDays(tarifa.DuracionDias); //TODO: deberia ser cada mes o cada año dependiendo de la tarifa, no necesariamente en base a dias
            var fechaProximoCobro = tarifa.PermiteAutoRenovacion == true
                ? fechaFin : (DateTimeOffset?)null;

            var proveedorPlan = new Entity.ProveedorPlan
            {
                IdProveedor = dto.IdProveedor,
                IdPlane = dto.IdPlane,
                IdPlanTarifa = dto.IdPlanTarifa,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                FechaProximoCobro = fechaProximoCobro,
                Estado = Constants.ESTADO_PROV_PLAN.PENDING,
                AutoRenovacion = tarifa.PermiteAutoRenovacion ?? false,
                EsActual = true,
                CulqiSubscriptionId = culqiResponse?.Id,
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

            var pagoPlan = new Entity.PagoPlan
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                Monto = monto,
                Moneda = Constants.CURRENCY.PEN,
                IdMetodoPago = metodoPago?.IdMetodoPago ?? 1,
                IdEstadoPago = estadoPendiente?.IdEstadoPago ?? 1,
                CulqiChargeId = culqiResponse?.Id,
                CodigoOperacion = culqiResponse?.ReferenceCode
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            response.AddOkResult("Suscripción iniciada. Espera la confirmación del webhook de Culqi.");
            return response;
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