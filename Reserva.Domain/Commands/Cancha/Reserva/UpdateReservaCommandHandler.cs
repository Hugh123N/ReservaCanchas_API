using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class UpdateReservaCommandHandler : CommandHandlerBase<UpdateReservaCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public UpdateReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateReservaCommandValidator validator,
            IRepository<Entity.Models.Reserva> ReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ReservaRepository = ReservaRepository;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(UpdateReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();
            var Reserva = await _ReservaRepository.GetByAsync(x => x.IdReserva == request.UpdateDto.IdReserva);

            if (Reserva != null)
            {
                _mapper?.Map(request.UpdateDto, Reserva);
                await _ReservaRepository.UpdateAsync(Reserva);
                await _ReservaRepository.SaveAsync();
            }

            var ReservaDto = _mapper?.Map<GetReservaDto>(Reserva);
            if (ReservaDto != null) response.UpdateData(ReservaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
