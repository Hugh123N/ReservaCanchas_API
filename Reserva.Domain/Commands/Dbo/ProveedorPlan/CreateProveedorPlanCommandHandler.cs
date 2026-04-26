using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CreateProveedorPlanCommandHandler : CommandHandlerBase<CreateProveedorPlanCommand, GetProveedorPlanDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public CreateProveedorPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateProveedorPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> ProveedorPlanRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;
        }

        public override async Task<ResponseDto<GetProveedorPlanDto>> HandleCommand(CreateProveedorPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorPlanDto>();

            var ProveedorPlan = _mapper?.Map<Entity.ProveedorPlan>(request.CreateDto);

            if (ProveedorPlan != null)
            {
                await _ProveedorPlanRepository.AddAsync(ProveedorPlan);
                await _ProveedorPlanRepository.SaveAsync();
            }

            var ProveedorPlanDto = _mapper?.Map<GetProveedorPlanDto>(ProveedorPlan);
            if (ProveedorPlanDto != null) response.UpdateData(ProveedorPlanDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}