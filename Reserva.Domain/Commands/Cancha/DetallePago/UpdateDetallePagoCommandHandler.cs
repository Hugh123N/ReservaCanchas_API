using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DetallePago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class UpdateDetallePagoCommandHandler : CommandHandlerBase<UpdateDetallePagoCommand, GetDetallePagoDto>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public UpdateDetallePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDetallePagoCommandValidator validator,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        public override async Task<ResponseDto<GetDetallePagoDto>> HandleCommand(UpdateDetallePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetallePagoDto>();
            var DetallePago = await _DetallePagoRepository.GetByAsync(x => x.IdDetallePago == request.UpdateDto.IdDetallePago);

            if (DetallePago != null)
            {
                _mapper?.Map(request.UpdateDto, DetallePago);
                await _DetallePagoRepository.UpdateAsync(DetallePago);
                await _DetallePagoRepository.SaveAsync();
            }

            var DetallePagoDto = _mapper?.Map<GetDetallePagoDto>(DetallePago);
            if (DetallePagoDto != null) response.UpdateData(DetallePagoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
