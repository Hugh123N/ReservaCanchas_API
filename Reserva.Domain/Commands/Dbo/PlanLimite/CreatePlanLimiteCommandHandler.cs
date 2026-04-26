using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PlanLimite;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PlanLimite
{
    public class CreatePlanLimiteCommandHandler : CommandHandlerBase<CreatePlanLimiteCommand, GetPlanLimiteDto>
    {
        private readonly IRepository<Entity.PlanLimite> _PlanLimiteRepository;

        public CreatePlanLimiteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreatePlanLimiteCommandValidator validator,
            IRepository<Entity.PlanLimite> PlanLimiteRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PlanLimiteRepository = PlanLimiteRepository;
        }

        public override async Task<ResponseDto<GetPlanLimiteDto>> HandleCommand(CreatePlanLimiteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlanLimiteDto>();

            var PlanLimite = _mapper?.Map<Entity.PlanLimite>(request.CreateDto);

            if (PlanLimite != null)
            {
                await _PlanLimiteRepository.AddAsync(PlanLimite);
                await _PlanLimiteRepository.SaveAsync();
            }

            var PlanLimiteDto = _mapper?.Map<GetPlanLimiteDto>(PlanLimite);
            if (PlanLimiteDto != null) response.UpdateData(PlanLimiteDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}