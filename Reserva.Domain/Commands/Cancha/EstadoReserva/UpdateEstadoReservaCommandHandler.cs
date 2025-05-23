using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class UpdateEstadoReservaCommandHandler : CommandHandlerBase<UpdateEstadoReservaCommand, GetEstadoReservaDto>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _EstadoReservaRepository;

        public UpdateEstadoReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateEstadoReservaCommandValidator validator,
            IRepository<Entity.Models.EstadoReserva> EstadoReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        public override async Task<ResponseDto<GetEstadoReservaDto>> HandleCommand(UpdateEstadoReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoReservaDto>();
            var EstadoReserva = await _EstadoReservaRepository.GetByAsync(x => x.IdEstadoReserva == request.UpdateDto.IdEstadoReserva);

            if (EstadoReserva != null)
            {
                _mapper?.Map(request.UpdateDto, EstadoReserva);
                await _EstadoReservaRepository.UpdateAsync(EstadoReserva);
                await _EstadoReservaRepository.SaveAsync();
            }

            var EstadoReservaDto = _mapper?.Map<GetEstadoReservaDto>(EstadoReserva);
            if (EstadoReservaDto != null) response.UpdateData(EstadoReservaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
