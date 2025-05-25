using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Pago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class UpdatePagoCommandHandler : CommandHandlerBase<UpdatePagoCommand, GetPagoDto>
    {
        private readonly IRepository<Entity.Models.Pago> _PagoRepository;

        public UpdatePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdatePagoCommandValidator validator,
            IRepository<Entity.Models.Pago> PagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PagoRepository = PagoRepository;
        }

        public override async Task<ResponseDto<GetPagoDto>> HandleCommand(UpdatePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoDto>();
            var Pago = await _PagoRepository.GetByAsync(x => x.IdPago == request.UpdateDto.IdPago);

            if (Pago != null)
            {
                _mapper?.Map(request.UpdateDto, Pago);
                await _PagoRepository.UpdateAsync(Pago);
                await _PagoRepository.SaveAsync();
            }

            var PagoDto = _mapper?.Map<GetPagoDto>(Pago);
            if (PagoDto != null) response.UpdateData(PagoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
