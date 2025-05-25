using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class UpdateMetodoPagoCommandHandler : CommandHandlerBase<UpdateMetodoPagoCommand, GetMetodoPagoDto>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public UpdateMetodoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateMetodoPagoCommandValidator validator,
            IRepository<Entity.Models.MetodoPago> MetodoPagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        public override async Task<ResponseDto<GetMetodoPagoDto>> HandleCommand(UpdateMetodoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetMetodoPagoDto>();
            var MetodoPago = await _MetodoPagoRepository.GetByAsync(x => x.IdMetodoPago == request.UpdateDto.IdMetodoPago);

            if (MetodoPago != null)
            {
                _mapper?.Map(request.UpdateDto, MetodoPago);
                await _MetodoPagoRepository.UpdateAsync(MetodoPago);
                await _MetodoPagoRepository.SaveAsync();
            }

            var MetodoPagoDto = _mapper?.Map<GetMetodoPagoDto>(MetodoPago);
            if (MetodoPagoDto != null) response.UpdateData(MetodoPagoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
