using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class UpdateEstadoPagoCommandHandler : CommandHandlerBase<UpdateEstadoPagoCommand, GetEstadoPagoDto>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public UpdateEstadoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateEstadoPagoCommandValidator validator,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        public override async Task<ResponseDto<GetEstadoPagoDto>> HandleCommand(UpdateEstadoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoPagoDto>();
            var EstadoPago = await _EstadoPagoRepository.GetByAsync(x => x.IdEstadoPago == request.UpdateDto.IdEstadoPago);

            if (EstadoPago != null)
            {
                _mapper?.Map(request.UpdateDto, EstadoPago);
                await _EstadoPagoRepository.UpdateAsync(EstadoPago);
                await _EstadoPagoRepository.SaveAsync();
            }

            var EstadoPagoDto = _mapper?.Map<GetEstadoPagoDto>(EstadoPago);
            if (EstadoPagoDto != null) response.UpdateData(EstadoPagoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
