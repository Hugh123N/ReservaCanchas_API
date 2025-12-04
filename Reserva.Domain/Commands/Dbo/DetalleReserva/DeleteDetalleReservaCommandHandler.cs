using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class DeleteDetalleReservaCommandHandler : CommandHandlerBase<DeleteDetalleReservaCommand>
    {
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public DeleteDetalleReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDetalleReservaCommandValidator validator,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DetalleReservaRepository = DetalleReservaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDetalleReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var DetalleReserva = await _DetalleReservaRepository.GetByAsync(x => x.IdDetalleReserva == request.Id);

            if (DetalleReserva != null)
            {
                DetalleReserva.Activo = false;
                await _DetalleReservaRepository.UpdateAsync(DetalleReserva);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
