using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class DeleteDetallePagoCommandHandler : CommandHandlerBase<DeleteDetallePagoCommand>
    {
        private readonly IRepository<Entity.Models.DetallePago> _DetallePagoRepository;

        public DeleteDetallePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDetallePagoCommandValidator validator,
            IRepository<Entity.Models.DetallePago> DetallePagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DetallePagoRepository = DetallePagoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDetallePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var DetallePago = await _DetallePagoRepository.GetByAsync(x => x.IdDetallePago == request.Id);

            if (DetallePago != null)
            {
                DetallePago.Activo = false;
                await _DetallePagoRepository.UpdateAsync(DetallePago);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
