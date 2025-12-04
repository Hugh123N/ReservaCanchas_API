using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class DeleteTipoSuperficieCommandHandler : CommandHandlerBase<DeleteTipoSuperficieCommand>
    {
        private readonly IRepository<Entity.TipoSuperficie> _TipoSuperficieRepository;

        public DeleteTipoSuperficieCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteTipoSuperficieCommandValidator validator,
            IRepository<Entity.TipoSuperficie> TipoSuperficieRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoSuperficieRepository = TipoSuperficieRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteTipoSuperficieCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var TipoSuperficie = await _TipoSuperficieRepository.GetByAsync(x => x.IdTipoSuperficie == request.Id);

            if (TipoSuperficie != null)
            {
                TipoSuperficie.Activo = false;
                await _TipoSuperficieRepository.UpdateAsync(TipoSuperficie);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
