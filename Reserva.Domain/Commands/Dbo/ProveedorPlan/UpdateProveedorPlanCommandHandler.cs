using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class UpdateProveedorPlanCommandHandler : CommandHandlerBase<UpdateProveedorPlanCommand, GetProveedorPlanDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public UpdateProveedorPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateProveedorPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> ProveedorPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;
        }

        public override async Task<ResponseDto<GetProveedorPlanDto>> HandleCommand(UpdateProveedorPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorPlanDto>();
            var ProveedorPlan = await _ProveedorPlanRepository.GetByAsync(x => x.IdProveedorPlan == request.UpdateDto.IdProveedorPlan);

            if (ProveedorPlan != null)
            {
                _mapper?.Map(request.UpdateDto, ProveedorPlan);
                await _ProveedorPlanRepository.UpdateAsync(ProveedorPlan);
                await _ProveedorPlanRepository.SaveAsync();
            }

            var ProveedorPlanDto = _mapper?.Map<GetProveedorPlanDto>(ProveedorPlan);
            if (ProveedorPlanDto != null) response.UpdateData(ProveedorPlanDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
