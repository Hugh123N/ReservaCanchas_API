using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Reserva
{
    public class DeleteReservaCommandHandler : CommandHandlerBase<DeleteReservaCommand>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public DeleteReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteReservaCommandValidator validator,
            IRepository<Entity.Models.Reserva> ReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ReservaRepository = ReservaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Reserva = await _ReservaRepository.GetByAsync(x => x.IdReserva == request.Id);

            if (Reserva != null)
            {
                Reserva.Activo = false;
                await _ReservaRepository.UpdateAsync(Reserva);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
