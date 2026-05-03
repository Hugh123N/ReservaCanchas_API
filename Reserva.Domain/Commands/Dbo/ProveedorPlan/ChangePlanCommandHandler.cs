using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Microsoft.EntityFrameworkCore;

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
            _culqiService = culqiService;
            _logger = logger;
        }

        public override async Task<ResponseDto<ChangePlanResponseDto>> HandleCommand(ChangePlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ChangePlanResponseDto>();
            var dto = request.ChangePlanDto;

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(x => x.IdProveedorPlan == dto.IdProveedorPlan);
            if (proveedorPlan == null)
            {
                response.AddErrorResult("Suscripción no encontrada");
                return response;
            }

            if (!proveedorPlan.EsActual || !proveedorPlan.Activo)
            {
                response.AddErrorResult("La suscripción no está activa");
                return response;
            }

            if (string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
            {
                response.AddErrorResult("La suscripción no tiene un ID de Culqi asociado");
                return response;
            }

            var nuevaTarifa = await _tarifaRepository.GetByAsync(x => x.IdPlanTarifa == dto.IdNuevaPlanTarifa, x => x.IdPlaneNavigation);
            if (nuevaTarifa == null)
            {
                response.AddErrorResult("Nueva tarifa no encontrada");
                return response;
            }

            decimal monto = nuevaTarifa.Precio;
            if (nuevaTarifa.PorcentajeDescuento.HasValue && nuevaTarifa.PorcentajeDescuento > 0)
            {
                monto = monto - (monto * nuevaTarifa.PorcentajeDescuento.Value / 100);
            }

            // Crear/obtener plan en Culqi para la nueva tarifa
            var nuevoCulqiPlanId = $"plan_{nuevaTarifa.IdPlanTarifa}";
            try
            {
                var existingPlan = await _culqiService.GetPlanAsync(nuevoCulqiPlanId);
                if (existingPlan == null)
                {
                    var planRequest = new CulqiCreatePlanRequest
                    {
                        Id = nuevoCulqiPlanId,
                        Name = $"{nuevaTarifa.IdPlaneNavigation?.Nombre} - {nuevaTarifa.Nombre}",
                        Amount = CulqiService.ConvertToCents(monto),
                         CurrencyCode = Constants.CURRENCY.PEN,
                        Interval = "months",
                        IntervalCount = nuevaTarifa.DuracionDias >= 30 ? nuevaTarifa.DuracionDias / 30 : 1,
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
                _logger.LogWarning("Error al crear plan en Culqi para cambio de plan: {Message}", ex.Message);
            }

            // Llamar a Culqi para actualizar la suscripción (prorrateo automático)
            CulqiSubscriptionResponse? culqiResponse;
            try
            {
                var updateRequest = new CulqiUpdateSubscriptionRequest
                {
                    PlanId = nuevoCulqiPlanId,
                    Metadata = new Dictionary<string, string>
                    {
                        { "plan_id", dto.IdNuevoPlane.ToString() },
                        { "tarifa_id", dto.IdNuevaPlanTarifa.ToString() },
                        { "tipo_cambio", "upgrade_downgrade" }
                    }
                };

                culqiResponse = await _culqiService.UpdateSubscriptionAsync(proveedorPlan.CulqiSubscriptionId, updateRequest);
            }
            catch (CulqiException ex)
            {
                response.AddErrorResult(ex.UserMessage ?? "Error al cambiar plan en Culqi");
                return response;
            }

            // Calcular nuevas fechas
            var fechaInicio = DateTimeOffset.UtcNow;
            var nuevaFechaFin = fechaInicio.AddDays(nuevaTarifa.DuracionDias);
            var nuevaFechaProximoCobro = nuevaTarifa.PermiteAutoRenovacion == true
                ? nuevaFechaFin : (DateTimeOffset?)null;

            // Actualizar ProveedorPlan
            proveedorPlan.IdPlane = dto.IdNuevoPlane;
            proveedorPlan.IdPlanTarifa = dto.IdNuevaPlanTarifa;
            proveedorPlan.FechaInicio = fechaInicio;
            proveedorPlan.FechaFin = nuevaFechaFin;
            proveedorPlan.FechaProximoCobro = nuevaFechaProximoCobro;
            proveedorPlan.Estado = Constants.ESTADO_PROV_PLAN.ACTIVE;
            proveedorPlan.AutoRenovacion = nuevaTarifa.PermiteAutoRenovacion ?? false;

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            // Registrar el cobro prorrateado en el historial
            var estadoPendiente = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pendiente);
            var metodoPago = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Yape);

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

            var changePlanResponse = new ChangePlanResponseDto
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                IdNuevoPlane = dto.IdNuevoPlane,
                IdNuevaPlanTarifa = dto.IdNuevaPlanTarifa,
                CulqiSubscriptionId = culqiResponse?.Id,
                MontoProrrateado = monto,
                Moneda = Constants.CURRENCY.PEN,
                Estado = Constants.ESTADO_PROV_PLAN.ACTIVE,
                NuevaFechaFin = nuevaFechaFin,
                NuevaFechaProximoCobro = nuevaFechaProximoCobro
            };

            response.UpdateData(changePlanResponse);
            response.AddOkResult("Plan cambiado exitosamente. Culqi aplicó el prorrateo correspondiente.");

            return response;
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
            });
        }
    }
}
