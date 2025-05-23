using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class DeleteDiaSemanaCommandHandler : CommandHandlerBase<DeleteDiaSemanaCommand>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public DeleteDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDiaSemanaCommandValidator validator,
            IRepository<Entity.Models.DiaSemana> DiaSemanaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DiaSemanaRepository = DiaSemanaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var DiaSemana = await _DiaSemanaRepository.GetByAsync(x => x.IdDiaSemana == request.Id);

            if (DiaSemana != null)
            {
                DiaSemana.Activo = false;
                await _DiaSemanaRepository.UpdateAsync(DiaSemana);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
