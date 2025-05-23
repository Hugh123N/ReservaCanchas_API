using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoPago
{
    public class DeleteEstadoPagoCommandHandler : CommandHandlerBase<DeleteEstadoPagoCommand>
    {
        private readonly IRepository<Entity.Models.EstadoPago> _EstadoPagoRepository;

        public DeleteEstadoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteEstadoPagoCommandValidator validator,
            IRepository<Entity.Models.EstadoPago> EstadoPagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoPagoRepository = EstadoPagoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteEstadoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var EstadoPago = await _EstadoPagoRepository.GetByAsync(x => x.IdEstadoPago == request.Id);

            if (EstadoPago != null)
            {
                EstadoPago.Activo = false;
                await _EstadoPagoRepository.UpdateAsync(EstadoPago);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
