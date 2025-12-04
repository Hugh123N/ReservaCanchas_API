using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class DeleteHorarioCanchaCommandHandler : CommandHandlerBase<DeleteHorarioCanchaCommand>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public DeleteHorarioCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteHorarioCanchaCommandValidator validator,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteHorarioCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var HorarioCancha = await _HorarioCanchaRepository.GetByAsync(x => x.IdHorarioCancha == request.Id);

            if (HorarioCancha != null)
            {
                HorarioCancha.Activo = false;
                await _HorarioCanchaRepository.UpdateAsync(HorarioCancha);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
