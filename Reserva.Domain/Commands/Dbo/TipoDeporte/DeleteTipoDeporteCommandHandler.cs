using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class DeleteTipoDeporteCommandHandler : CommandHandlerBase<DeleteTipoDeporteCommand>
    {
        private readonly IRepository<Entity.TipoDeporte> _TipoDeporteRepository;

        public DeleteTipoDeporteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteTipoDeporteCommandValidator validator,
            IRepository<Entity.TipoDeporte> TipoDeporteRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _TipoDeporteRepository = TipoDeporteRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteTipoDeporteCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var TipoDeporte = await _TipoDeporteRepository.GetByAsync(x => x.IdTipoDeporte == request.Id);

            if (TipoDeporte != null)
            {
                TipoDeporte.Activo = false;
                await _TipoDeporteRepository.UpdateAsync(TipoDeporte);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
