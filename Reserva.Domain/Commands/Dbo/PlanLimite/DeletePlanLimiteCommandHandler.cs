using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class DeletePlanLimiteCommandHandler : CommandHandlerBase<DeletePlanLimiteCommand>
    {
        private readonly IRepository<Entity.PlanLimite> _PlanLimiteRepository;

        public DeletePlanLimiteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeletePlanLimiteCommandValidator validator,
            IRepository<Entity.PlanLimite> PlanLimiteRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PlanLimiteRepository = PlanLimiteRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeletePlanLimiteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var PlanLimite = await _PlanLimiteRepository.GetByAsync(x => x.IdPlanLimite == request.Id);

            if (PlanLimite != null)
            {
                PlanLimite.Activo = false;
                await _PlanLimiteRepository.UpdateAsync(PlanLimite);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
