using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class DeleteDisponibilidadCommandHandler : CommandHandlerBase<DeleteDisponibilidadCommand>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public DeleteDisponibilidadCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDisponibilidadCommandValidator validator,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDisponibilidadCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Disponibilidad = await _DisponibilidadRepository.GetByAsync(x => x.IdDisponibilidad == request.Id);

            if (Disponibilidad != null)
            {
                Disponibilidad.Activo = false;
                await _DisponibilidadRepository.UpdateAsync(Disponibilidad);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
