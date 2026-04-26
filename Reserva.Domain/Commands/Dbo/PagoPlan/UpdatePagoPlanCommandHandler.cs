using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class UpdatePagoPlanCommandHandler : CommandHandlerBase<UpdatePagoPlanCommand, GetPagoPlanDto>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public UpdatePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdatePagoPlanCommandValidator validator,
            IRepository<Entity.PagoPlan> PagoPlanRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PagoPlanRepository = PagoPlanRepository;
        }

        public override async Task<ResponseDto<GetPagoPlanDto>> HandleCommand(UpdatePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoPlanDto>();
            var PagoPlan = await _PagoPlanRepository.GetByAsync(x => x.IdPagoPlan == request.UpdateDto.IdPagoPlan);

            if (PagoPlan != null)
            {
                _mapper?.Map(request.UpdateDto, PagoPlan);
                await _PagoPlanRepository.UpdateAsync(PagoPlan);
                await _PagoPlanRepository.SaveAsync();
            }

            var PagoPlanDto = _mapper?.Map<GetPagoPlanDto>(PagoPlan);
            if (PagoPlanDto != null) response.UpdateData(PagoPlanDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
