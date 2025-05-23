using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class CreateEstadoPagoCommandHandler : CommandHandlerBase<CreateEstadoPagoCommand, GetEstadoPagoDto>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public CreateEstadoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateEstadoPagoCommandValidator validator,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        public override async Task<ResponseDto<GetEstadoPagoDto>> HandleCommand(CreateEstadoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoPagoDto>();

            var EstadoPago = _mapper?.Map<Entity.Models.EstadoPago>(request.CreateDto);

            if (EstadoPago != null)
            {
                await _EstadoPagoRepository.AddAsync(EstadoPago);
                await _EstadoPagoRepository.SaveAsync();
            }

            var EstadoPagoDto = _mapper?.Map<GetEstadoPagoDto>(EstadoPago);
            if (EstadoPagoDto != null) response.UpdateData(EstadoPagoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}