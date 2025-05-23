using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class DeleteZonaCommandHandler : CommandHandlerBase<DeleteZonaCommand>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public DeleteZonaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteZonaCommandValidator validator,
            IRepository<Entity.Models.Zona> ZonaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ZonaRepository = ZonaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteZonaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Zona = await _ZonaRepository.GetByAsync(x => x.IdZona == request.Id);

            if (Zona != null)
            {
                Zona.Activo = false;
                await _ZonaRepository.UpdateAsync(Zona);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
