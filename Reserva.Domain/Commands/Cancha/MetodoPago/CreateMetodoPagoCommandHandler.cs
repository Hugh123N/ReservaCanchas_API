using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class CreateMetodoPagoCommandHandler : CommandHandlerBase<CreateMetodoPagoCommand, GetMetodoPagoDto>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public CreateMetodoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateMetodoPagoCommandValidator validator,
            IRepository<Entity.Models.MetodoPago> MetodoPagoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        public override async Task<ResponseDto<GetMetodoPagoDto>> HandleCommand(CreateMetodoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetMetodoPagoDto>();

            var MetodoPago = _mapper?.Map<Entity.Models.MetodoPago>(request.CreateDto);

            if (MetodoPago != null)
            {
                await _MetodoPagoRepository.AddAsync(MetodoPago);
                await _MetodoPagoRepository.SaveAsync();
            }

            var MetodoPagoDto = _mapper?.Map<GetMetodoPagoDto>(MetodoPago);
            if (MetodoPagoDto != null) response.UpdateData(MetodoPagoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}