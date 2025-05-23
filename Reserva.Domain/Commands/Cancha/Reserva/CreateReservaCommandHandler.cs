using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class CreateReservaCommandHandler : CommandHandlerBase<CreateReservaCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public CreateReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateReservaCommandValidator validator,
            IRepository<Entity.Models.Reserva> ReservaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(CreateReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            var Reserva = _mapper?.Map<Entity.Models.Reserva>(request.CreateDto);

            if (Reserva != null)
            {
                await _ReservaRepository.AddAsync(Reserva);
                await _ReservaRepository.SaveAsync();
            }

            var ReservaDto = _mapper?.Map<GetReservaDto>(Reserva);
            if (ReservaDto != null) response.UpdateData(ReservaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}