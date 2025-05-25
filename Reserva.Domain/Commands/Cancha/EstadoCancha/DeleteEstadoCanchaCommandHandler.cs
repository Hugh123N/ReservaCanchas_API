using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class DeleteEstadoCanchaCommandHandler : CommandHandlerBase<DeleteEstadoCanchaCommand>
    {
        private readonly IRepository<Entity.Models.EstadoCancha> _EstadoCanchaRepository;

        public DeleteEstadoCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteEstadoCanchaCommandValidator validator,
            IRepository<Entity.Models.EstadoCancha> EstadoCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoCanchaRepository = EstadoCanchaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteEstadoCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var EstadoCancha = await _EstadoCanchaRepository.GetByAsync(x => x.IdEstadoCancha == request.Id);

            if (EstadoCancha != null)
            {
                EstadoCancha.Activo = false;
                await _EstadoCanchaRepository.UpdateAsync(EstadoCancha);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
