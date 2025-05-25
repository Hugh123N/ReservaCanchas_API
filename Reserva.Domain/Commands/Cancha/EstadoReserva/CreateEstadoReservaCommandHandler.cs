using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class CreateEstadoReservaCommandHandler : CommandHandlerBase<CreateEstadoReservaCommand, GetEstadoReservaDto>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _EstadoReservaRepository;

        public CreateEstadoReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateEstadoReservaCommandValidator validator,
            IRepository<Entity.Models.EstadoReserva> EstadoReservaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        public override async Task<ResponseDto<GetEstadoReservaDto>> HandleCommand(CreateEstadoReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoReservaDto>();

            var EstadoReserva = _mapper?.Map<Entity.Models.EstadoReserva>(request.CreateDto);

            if (EstadoReserva != null)
            {
                await _EstadoReservaRepository.AddAsync(EstadoReserva);
                await _EstadoReservaRepository.SaveAsync();
            }

            var EstadoReservaDto = _mapper?.Map<GetEstadoReservaDto>(EstadoReserva);
            if (EstadoReservaDto != null) response.UpdateData(EstadoReservaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}