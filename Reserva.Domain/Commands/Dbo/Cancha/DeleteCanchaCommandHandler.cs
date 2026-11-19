using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Domain.Services.GestionAdmin;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class DeleteCanchaCommandHandler : CommandHandlerBase<DeleteCanchaCommand>
    {
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly NegocioAccessService _access;

        public DeleteCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteCanchaCommandValidator validator,
            IRepository<Entity.Cancha> CanchaRepository,
            NegocioAccessService access
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
            _access = access;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.Id);
            if (Cancha == null || !await _access.PuedeEditarCanchaAsync(Cancha))
            { response.AddErrorResult("No tienes permiso para eliminar esta cancha."); return response; }

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
