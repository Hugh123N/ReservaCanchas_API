using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class RetryPaymentPlanCommandHandler : CommandHandlerBase<RetryPaymentPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.MetodoPago> _metodoPagoRepository;
        private readonly IRepository<Entity.PagoPlan> _pagoPlanRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly ICulqiService _culqiService;
        private readonly ILogger<RetryPaymentPlanCommandHandler> _logger;

        public RetryPaymentPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            RetryPaymentPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.PlanTarifa> tarifaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.MetodoPago> metodoPagoRepository,
            IRepository<Entity.PagoPlan> pagoPlanRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            ICulqiService culqiService,
            ILogger<RetryPaymentPlanCommandHandler> logger
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _tarifaRepository = tarifaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _metodoPagoRepository = metodoPagoRepository;
            _pagoPlanRepository = pagoPlanRepository;
            _proveedorRepository = proveedorRepository;
            _culqiService = culqiService;
            _logger = logger;
        }

        public override async Task<ResponseDto> HandleCommand(RetryPaymentPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var dto = request.RetryPaymentDto;

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

            if (proveedorPlan.Estado != Constants.ESTADO_PROV_PLAN.GRACE && proveedorPlan.Estado != Constants.ESTADO_PROV_PLAN.PAST_DUE)
            {
                response.AddErrorResult("La suscripción no está en estado de mora. No se puede reintentar el pago.");
                return response;
            }

            var tarifa = proveedorPlan.IdPlanTarifaNavigation;
            decimal monto = tarifa?.Precio ?? 0;

            if (tarifa?.PorcentajeDescuento.HasValue == true && tarifa.PorcentajeDescuento > 0)
            {
                monto = monto - (monto * tarifa.PorcentajeDescuento.Value / 100);
            }

            var proveedor = await _proveedorRepository.GetByAsync(
                x => x.IdProveedor == proveedorPlan.IdProveedor,
                x => x.IdUsuarioNavigation
            );

            if (proveedor == null)
            {
                response.AddErrorResult("Proveedor no encontrado");
                return response;
            }

            // Si se proporcionó un token, actualizar la tarjeta del cliente
            if (!string.IsNullOrEmpty(dto.CulqiToken) && !string.IsNullOrEmpty(proveedor.CulqiCustomerId))
            {
                try
                {
                    var existingCard = await _culqiService.GetCardAsync(proveedor.CulqiCustomerId);

                    var newCard = await _culqiService.CreateCardAsync(proveedor.CulqiCustomerId, dto.CulqiToken);

                    if (existingCard != null)
                    {
                        await _culqiService.DeleteCardAsync(existingCard.Id);
                        _logger.LogInformation("Tarjeta anterior eliminada: {CardId}", existingCard.Id);
                    }

                    // Actualizar suscripción con la nueva tarjeta
                    if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
                    {
                        var updateRequest = new CulqiUpdateSubscriptionRequest
                        {
                            CardId = newCard.Id
                        };

                        await _culqiService.UpdateSubscriptionAsync(proveedorPlan.CulqiSubscriptionId, updateRequest);
                        _logger.LogInformation("Suscripción {SubscriptionId} actualizada con nueva tarjeta", proveedorPlan.CulqiSubscriptionId);
                    }
                }
                catch (CulqiException ex)
                {
                    _logger.LogError(ex, "Error al actualizar tarjeta en Culqi");
                    response.AddErrorResult(ex.UserMessage ?? "Error al actualizar la tarjeta");
                    return response;
                }
            }

            // Registrar el pago
            var estadoPendiente = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pendiente);
            var metodoPago = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Yape);

            var pagoPlan = new Entity.PagoPlan
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                Monto = monto,
                Moneda = Constants.CURRENCY.PEN,
                IdMetodoPago = metodoPago?.IdMetodoPago ?? 1,
                IdEstadoPago = estadoPendiente?.IdEstadoPago ?? 1,
                CulqiChargeId = proveedorPlan.CulqiSubscriptionId,
                CodigoOperacion = null,
                Activo = true
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            _logger.LogInformation("Pago registrado para ProveedorPlan {IdProveedorPlan}. Culqi procesará el cobro automáticamente.", proveedorPlan.IdProveedorPlan);

            response.AddOkResult("Pago registrado. Culqi procesará el cobro automáticamente.");
            return response;
        }
    }

    public class RetryPaymentPlanCommandValidator : CommandValidatorBase<RetryPaymentPlanCommand>
    {
        public RetryPaymentPlanCommandValidator()
        {
            RequiredInformation(x => x.RetryPaymentDto).DependentRules(() =>
            {
                RuleFor(x => x.RetryPaymentDto.IdProveedorPlan)
                    .GreaterThan(0)
                    .WithMessage("La suscripción es requerida");

                RuleFor(x => x.RetryPaymentDto.Email)
                    .EmailAddress()
                    .When(x => !string.IsNullOrEmpty(x.RetryPaymentDto.CulqiToken))
                    .WithMessage("El email es requerido cuando se envía un token de tarjeta");
            });
        }
    }
}
