using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class UpdateComprobantePagoPlanCommandHandler : CommandHandlerBase<UpdateComprobantePagoPlanCommand, GetComprobantePagoPlanDto>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public UpdateComprobantePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateComprobantePagoPlanCommandValidator validator,
            IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;
        }

        public override async Task<ResponseDto<GetComprobantePagoPlanDto>> HandleCommand(UpdateComprobantePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComprobantePagoPlanDto>();
            var ComprobantePagoPlan = await _ComprobantePagoPlanRepository.GetByAsync(x => x.IdComprobantePagoPlan == request.UpdateDto.IdComprobantePagoPlan);

            if (ComprobantePagoPlan != null)
            {
                _mapper?.Map(request.UpdateDto, ComprobantePagoPlan);
                await _ComprobantePagoPlanRepository.UpdateAsync(ComprobantePagoPlan);
                await _ComprobantePagoPlanRepository.SaveAsync();
            }

            var ComprobantePagoPlanDto = _mapper?.Map<GetComprobantePagoPlanDto>(ComprobantePagoPlan);
            if (ComprobantePagoPlanDto != null) response.UpdateData(ComprobantePagoPlanDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
