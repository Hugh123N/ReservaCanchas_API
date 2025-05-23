using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class CreateDetallePagoCommandHandler : CommandHandlerBase<CreateDetallePagoCommand, GetDetallePagoDto>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public CreateDetallePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateDetallePagoCommandValidator validator,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        public override async Task<ResponseDto<GetDetallePagoDto>> HandleCommand(CreateDetallePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetallePagoDto>();

            var DetallePago = _mapper?.Map<Entity.Models.DetallePago>(request.CreateDto);

            if (DetallePago != null)
            {
                await _DetallePagoRepository.AddAsync(DetallePago);
                await _DetallePagoRepository.SaveAsync();
            }

            var DetallePagoDto = _mapper?.Map<GetDetallePagoDto>(DetallePago);
            if (DetallePagoDto != null) response.UpdateData(DetallePagoDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}