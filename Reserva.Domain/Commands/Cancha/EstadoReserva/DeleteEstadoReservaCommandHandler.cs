using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoReserva
{
    public class DeleteEstadoReservaCommandHandler : CommandHandlerBase<DeleteEstadoReservaCommand>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _EstadoReservaRepository;

        public DeleteEstadoReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteEstadoReservaCommandValidator validator,
            IRepository<Entity.Models.EstadoReserva> EstadoReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteEstadoReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var EstadoReserva = await _EstadoReservaRepository.GetByAsync(x => x.IdEstadoReserva == request.Id);

            if (EstadoReserva != null)
            {
                EstadoReserva.Activo = false;
                await _EstadoReservaRepository.UpdateAsync(EstadoReserva);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
