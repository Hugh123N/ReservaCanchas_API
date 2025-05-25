using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.MetodoPago
{
    public class DeleteMetodoPagoCommandHandler : CommandHandlerBase<DeleteMetodoPagoCommand>
    {
        private readonly IRepository<Entity.Models.MetodoPago> _MetodoPagoRepository;

        public DeleteMetodoPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteMetodoPagoCommandValidator validator,
            IRepository<Entity.Models.MetodoPago> MetodoPagoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _MetodoPagoRepository = MetodoPagoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteMetodoPagoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var MetodoPago = await _MetodoPagoRepository.GetByAsync(x => x.IdMetodoPago == request.Id);

            if (MetodoPago != null)
            {
                MetodoPago.Activo = false;
                await _MetodoPagoRepository.UpdateAsync(MetodoPago);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
