using AutoMapper;
using MediatR;
using Reserva.Common;
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

            // Validar que el plan esté activo y tenga renovación automática
            if (proveedorPlan.Estado != Constants.ESTADO_PROV_PLAN.ACTIVE)
            {
                response.AddErrorResult("Solo se puede cancelar la renovación de planes activos");
                return response;
            }

            if (!proveedorPlan.AutoRenovacion)
            {
                response.AddErrorResult("La renovación automática ya está desactivada");
                return response;
            }

            // Cancelar suscripción en Culqi (permanentemente)
            if (!string.IsNullOrEmpty(proveedorPlan.CulqiSubscriptionId))
            {
                var culqiResult = await _culqiService.CancelSubscriptionAsync(proveedorPlan.CulqiSubscriptionId);
                if (!culqiResult)
                {
                    response.AddErrorResult("Error al cancelar la suscripción en Culqi");
                    return response;
                }
            }

            // El plan permanece ACTIVE hasta FechaFin, pero con renovación cancelada
            proveedorPlan.CancelAtPeriodEnd = true;
            proveedorPlan.MotivoCancelacion = "Cancelado Autorenovacion por el proveedor.";

            await _proveedorPlanRepository.UpdateAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            response.AddOkResult("Renovación automática cancelada correctamente. El plan permanece activo hasta " + proveedorPlan.FechaFin.ToString("dd/MM/yyyy"));
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