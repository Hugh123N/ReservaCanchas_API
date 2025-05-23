using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class DeleteTipoCanchaCommandHandler : CommandHandlerBase<DeleteTipoCanchaCommand>
    {
        private readonly IRepository<Entity.Models.TipoCancha> _TipoCanchaRepository;

        public DeleteTipoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteTipoCanchaCommandValidator validator,
            IRepository<Entity.Models.TipoCancha> TipoCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoCanchaRepository = TipoCanchaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteTipoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var TipoCancha = await _TipoCanchaRepository.GetByAsync(x => x.IdTipoCancha == request.Id);

            if (TipoCancha != null)
            {
                TipoCancha.Activo = false;
                await _TipoCanchaRepository.UpdateAsync(TipoCancha);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
