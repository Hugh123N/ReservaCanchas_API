using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class DeleteProveedorPlanCommandHandler : CommandHandlerBase<DeleteProveedorPlanCommand>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public DeleteProveedorPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteProveedorPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> ProveedorPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteProveedorPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var ProveedorPlan = await _ProveedorPlanRepository.GetByAsync(x => x.IdProveedorPlan == request.Id);

            if (ProveedorPlan != null)
            {
                ProveedorPlan.Activo = false;
                await _ProveedorPlanRepository.UpdateAsync(ProveedorPlan);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
