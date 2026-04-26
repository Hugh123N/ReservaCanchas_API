using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class CreatePagoPlanCommandHandler : CommandHandlerBase<CreatePagoPlanCommand, GetPagoPlanDto>
    {
        private readonly IRepository<Entity.PagoPlan> _PagoPlanRepository;

        public CreatePagoPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreatePagoPlanCommandValidator validator,
            IRepository<Entity.PagoPlan> PagoPlanRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PagoPlanRepository = PagoPlanRepository;
        }

        public override async Task<ResponseDto<GetPagoPlanDto>> HandleCommand(CreatePagoPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoPlanDto>();

            var PagoPlan = _mapper?.Map<Entity.PagoPlan>(request.CreateDto);

            if (PagoPlan != null)
            {
                await _PagoPlanRepository.AddAsync(PagoPlan);
                await _PagoPlanRepository.SaveAsync();
            }

            var PagoPlanDto = _mapper?.Map<GetPagoPlanDto>(PagoPlan);
            if (PagoPlanDto != null) response.UpdateData(PagoPlanDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}