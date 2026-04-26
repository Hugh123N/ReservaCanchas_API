using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PlanLimite;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class UpdatePlanLimiteCommandHandler : CommandHandlerBase<UpdatePlanLimiteCommand, GetPlanLimiteDto>
    {
        private readonly IRepository<Entity.PlanLimite> _PlanLimiteRepository;

        public UpdatePlanLimiteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdatePlanLimiteCommandValidator validator,
            IRepository<Entity.PlanLimite> PlanLimiteRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PlanLimiteRepository = PlanLimiteRepository;
        }

        public override async Task<ResponseDto<GetPlanLimiteDto>> HandleCommand(UpdatePlanLimiteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlanLimiteDto>();
            var PlanLimite = await _PlanLimiteRepository.GetByAsync(x => x.IdPlanLimite == request.UpdateDto.IdPlanLimite);

            if (PlanLimite != null)
            {
                _mapper?.Map(request.UpdateDto, PlanLimite);
                await _PlanLimiteRepository.UpdateAsync(PlanLimite);
                await _PlanLimiteRepository.SaveAsync();
            }

            var PlanLimiteDto = _mapper?.Map<GetPlanLimiteDto>(PlanLimite);
            if (PlanLimiteDto != null) response.UpdateData(PlanLimiteDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
