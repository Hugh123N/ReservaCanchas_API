using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CancelAutoRenewCommandHandler : CommandHandlerBase<CancelAutoRenewCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly ICulqiService _culqiService;

        public CancelAutoRenewCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CancelAutoRenewCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            ICulqiService culqiService
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _culqiService = culqiService;
        }

        public override async Task<ResponseDto> HandleCommand(CancelAutoRenewCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(x => x.IdProveedorPlan == request.IdProveedorPlan);
            if (proveedorPlan == null)
            {
                response.AddErrorResult("Suscripción no encontrada");
                return response;
            }

            // Cancelar suscripción en Culqi si existe
            if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
            {
                await _culqiService.CancelSubscriptionAsync(proveedorPlan.CulqiSubscriptionId);
            }

            proveedorPlan.AutoRenovacion = false;
            proveedorPlan.FechaCancelacion = DateTimeOffset.UtcNow;
            proveedorPlan.MotivoCancelacion = "Cancelado por el proveedor";

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            response.AddOkResult("Renovación automática cancelada correctamente");
            return response;
        }
    }

    public class CancelAutoRenewCommandValidator : CommandValidatorBase<CancelAutoRenewCommand>
    {
        // public CancelAutoRenewCommandValidator()
        // {
        //     ValidateId(x => x.IdProveedorPlan, "Suscripción");
        // }
    }
}