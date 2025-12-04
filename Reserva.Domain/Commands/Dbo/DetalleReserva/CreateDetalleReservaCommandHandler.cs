using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class CreateDetalleReservaCommandHandler : CommandHandlerBase<CreateDetalleReservaCommand, GetDetalleReservaDto>
    {
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public CreateDetalleReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDetalleReservaCommandValidator validator,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DetalleReservaRepository = DetalleReservaRepository;
        }

        public override async Task<ResponseDto<GetDetalleReservaDto>> HandleCommand(CreateDetalleReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetalleReservaDto>();

            var DetalleReserva = _mapper?.Map<Entity.DetalleReserva>(request.CreateDto);

            if (DetalleReserva != null)
            {
                await _DetalleReservaRepository.AddAsync(DetalleReserva);
                await _DetalleReservaRepository.SaveAsync();
            }

            var DetalleReservaDto = _mapper?.Map<GetDetalleReservaDto>(DetalleReserva);
            if (DetalleReservaDto != null) response.UpdateData(DetalleReservaDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}