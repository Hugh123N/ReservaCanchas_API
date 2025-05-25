using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Pago
{
    public class DeletePagoCommandHandler : CommandHandlerBase<DeletePagoCommand>
    {
        private readonly IRepository<Entity.Models.Pago> _PagoRepository;

        public DeletePagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeletePagoCommandValidator validator,
            IRepository<Entity.Models.Pago> PagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PagoRepository = PagoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeletePagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Pago = await _PagoRepository.GetByAsync(x => x.IdPago == request.Id);

            if (Pago != null)
            {
                Pago.Activo = false;
                await _PagoRepository.UpdateAsync(Pago);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
