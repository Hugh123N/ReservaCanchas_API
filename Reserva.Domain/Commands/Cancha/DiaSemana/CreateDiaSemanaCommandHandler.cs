using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class CreateDiaSemanaCommandHandler : CommandHandlerBase<CreateDiaSemanaCommand, GetDiaSemanaDto>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public CreateDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDiaSemanaCommandValidator validator,
            IRepository<Entity.Models.DiaSemana> DiaSemanaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DiaSemanaRepository = DiaSemanaRepository;
        }

        public override async Task<ResponseDto<GetDiaSemanaDto>> HandleCommand(CreateDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();

            var DiaSemana = _mapper?.Map<Entity.Models.DiaSemana>(request.CreateDto);

            if (DiaSemana != null)
            {
                await _DiaSemanaRepository.AddAsync(DiaSemana);
                await _DiaSemanaRepository.SaveAsync();
            }

            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);
            if (DiaSemanaDto != null) response.UpdateData(DiaSemanaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}