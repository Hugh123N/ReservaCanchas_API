using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class CreateHorarioCanchaCommandHandler : CommandHandlerBase<CreateHorarioCanchaCommand, GetHorarioCanchaDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public CreateHorarioCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateHorarioCanchaCommandValidator validator,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
        }

        public override async Task<ResponseDto<GetHorarioCanchaDto>> HandleCommand(CreateHorarioCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetHorarioCanchaDto>();

            var HorarioCancha = _mapper?.Map<Entity.HorarioCancha>(request.CreateDto);

            if (HorarioCancha != null)
            {
                await _HorarioCanchaRepository.AddAsync(HorarioCancha);
                await _HorarioCanchaRepository.SaveAsync();
            }

            var HorarioCanchaDto = _mapper?.Map<GetHorarioCanchaDto>(HorarioCancha);
            if (HorarioCanchaDto != null) response.UpdateData(HorarioCanchaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}