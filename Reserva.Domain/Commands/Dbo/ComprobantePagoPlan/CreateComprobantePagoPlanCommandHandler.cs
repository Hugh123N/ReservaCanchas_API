using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class CreateComprobantePagoPlanCommandHandler : CommandHandlerBase<CreateComprobantePagoPlanCommand, GetComprobantePagoPlanDto>
    {
        private readonly IRepository<Entity.ComprobantePagoPlan> _ComprobantePagoPlanRepository;

        public CreateComprobantePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateComprobantePagoPlanCommandValidator validator,
            IRepository<Entity.ComprobantePagoPlan> ComprobantePagoPlanRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ComprobantePagoPlanRepository = ComprobantePagoPlanRepository;
        }

        public override async Task<ResponseDto<GetComprobantePagoPlanDto>> HandleCommand(CreateComprobantePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComprobantePagoPlanDto>();

            var ComprobantePagoPlan = _mapper?.Map<Entity.ComprobantePagoPlan>(request.CreateDto);

            if (ComprobantePagoPlan != null)
            {
                await _ComprobantePagoPlanRepository.AddAsync(ComprobantePagoPlan);
                await _ComprobantePagoPlanRepository.SaveAsync();
            }

            var ComprobantePagoPlanDto = _mapper?.Map<GetComprobantePagoPlanDto>(ComprobantePagoPlan);
            if (ComprobantePagoPlanDto != null) response.UpdateData(ComprobantePagoPlanDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}