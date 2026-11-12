using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Domain.Services.Pago;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class DeleteReservaCommandHandler : CommandHandlerBase<DeleteReservaCommand>
    {
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly PagoReservaAccessService _access;

        public DeleteReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteReservaCommandValidator validator,
            IRepository<Entity.Reserva> ReservaRepository,
            PagoReservaAccessService access
        ) : base(unitOfWork, mapper, validator)
        {
            _ReservaRepository = ReservaRepository;
            _access = access;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Reserva = await _ReservaRepository.GetByAsync(x => x.IdReserva == request.Id && x.Activo,
                r => r.IdCanchaNavigation, r => r.Pago.Where(p => p.Activo));

            if (Reserva != null)
            {
                if (!await _access.PuedeAccederAsync(Reserva))
                {
                    response.AddErrorResult("No tienes acceso a esta reserva.");
                    return response;
                }
                if (Reserva.Pago.Any(p => p.MontoAdelanto > 0))
                {
                    response.AddErrorResult("La reserva tiene cobros. Usa cancelar con reembolso para conservar el control del dinero.");
                    return response;
                }
                foreach (var pago in Reserva.Pago) pago.Revision++;
                Reserva.Activo = false;
                await _ReservaRepository.UpdateAsync(Reserva);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
