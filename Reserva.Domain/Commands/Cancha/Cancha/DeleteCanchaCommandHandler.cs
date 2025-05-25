using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class DeleteCanchaCommandHandler : CommandHandlerBase<DeleteCanchaCommand>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public DeleteCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteCanchaCommandValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.Id);

            if (Cancha != null)
            {
                Cancha.Activo = false;
                await _CanchaRepository.UpdateAsync(Cancha);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
