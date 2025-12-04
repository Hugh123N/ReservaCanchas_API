using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class UpdateDetalleReservaCommandHandler : CommandHandlerBase<UpdateDetalleReservaCommand, GetDetalleReservaDto>
    {
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public UpdateDetalleReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDetalleReservaCommandValidator validator,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DetalleReservaRepository = DetalleReservaRepository;
        }

        public override async Task<ResponseDto<GetDetalleReservaDto>> HandleCommand(UpdateDetalleReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDetalleReservaDto>();
            var DetalleReserva = await _DetalleReservaRepository.GetByAsync(x => x.IdDetalleReserva == request.UpdateDto.IdDetalleReserva);

            if (DetalleReserva != null)
            {
                _mapper?.Map(request.UpdateDto, DetalleReserva);
                await _DetalleReservaRepository.UpdateAsync(DetalleReserva);
                await _DetalleReservaRepository.SaveAsync();
            }

            var DetalleReservaDto = _mapper?.Map<GetDetalleReservaDto>(DetalleReserva);
            if (DetalleReservaDto != null) response.UpdateData(DetalleReservaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
