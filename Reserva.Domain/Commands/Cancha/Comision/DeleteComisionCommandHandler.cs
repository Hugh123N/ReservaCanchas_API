using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class DeleteComisionCommandHandler : CommandHandlerBase<DeleteComisionCommand>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public DeleteComisionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteComisionCommandValidator validator,
            IRepository<Entity.Models.Comision> ComisionRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ComisionRepository = ComisionRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteComisionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Comision = await _ComisionRepository.GetByAsync(x => x.IdComision == request.Id);

            if (Comision != null)
            {
                Comision.Activo = false;
                await _ComisionRepository.UpdateAsync(Comision);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
