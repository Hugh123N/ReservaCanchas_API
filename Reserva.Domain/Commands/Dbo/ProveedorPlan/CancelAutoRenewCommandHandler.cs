using AutoMapper;
using MediatR;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CancelAutoRenewCommandHandler : CommandHandlerBase<CancelAutoRenewCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;

        public CancelAutoRenewCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CancelAutoRenewCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
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