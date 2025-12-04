using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class DeleteServicioCommandHandler : CommandHandlerBase<DeleteServicioCommand>
    {
        private readonly IRepository<Entity.Servicio> _ServicioRepository;

        public DeleteServicioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteServicioCommandValidator validator,
            IRepository<Entity.Servicio> ServicioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ServicioRepository = ServicioRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteServicioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Servicio = await _ServicioRepository.GetByAsync(x => x.IdServicio == request.Id);

            if (Servicio != null)
            {
                Servicio.Activo = false;
                await _ServicioRepository.UpdateAsync(Servicio);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
