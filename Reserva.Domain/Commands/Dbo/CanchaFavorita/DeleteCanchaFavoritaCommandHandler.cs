using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.CanchaFavorita
{
    public class DeleteCanchaFavoritaCommandHandler : CommandHandlerBase<DeleteCanchaFavoritaCommand>
    {
        private readonly IRepository<Entity.CanchaFavorita> _CanchaFavoritaRepository;

        public DeleteCanchaFavoritaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteCanchaFavoritaCommandValidator validator,
            IRepository<Entity.CanchaFavorita> CanchaFavoritaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaFavoritaRepository = CanchaFavoritaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteCanchaFavoritaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            if (!Guid.TryParse(request.IdUsuario, out var idUsuarioGuid))
            {
                response.AddErrorResult("El IdUsuario proporcionado no es un GUID válido.");
                return response;
            }

            var CanchaFavorita = await _CanchaFavoritaRepository.GetByAsync(x => x.IdCancha == request.Id && x.IdUsuario == idUsuarioGuid);

            if (CanchaFavorita != null)
            {
                CanchaFavorita.Activo = false;
                await _CanchaFavoritaRepository.UpdateAsync(CanchaFavorita);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
