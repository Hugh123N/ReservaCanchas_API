using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CreatePagoCommandHandler : CommandHandlerBase<CreatePagoCommand, GetPagoDto>
    {
        private readonly IRepository<Entity.Pago> _PagoRepository;

        public CreatePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreatePagoCommandValidator validator,
            IRepository<Entity.Pago> PagoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PagoRepository = PagoRepository;
        }

        public override async Task<ResponseDto<GetPagoDto>> HandleCommand(CreatePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoDto>();

            var Pago = _mapper?.Map<Entity.Pago>(request.CreateDto);

            if (Pago != null)
            {
                await _PagoRepository.AddAsync(Pago);
                await _PagoRepository.SaveAsync();
            }

            var PagoDto = _mapper?.Map<GetPagoDto>(Pago);
            if (PagoDto != null) response.UpdateData(PagoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}