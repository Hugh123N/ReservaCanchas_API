using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class DeleteDistritoCommandHandler : CommandHandlerBase<DeleteDistritoCommand>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public DeleteDistritoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteDistritoCommandValidator validator,
            IRepository<Entity.Models.Distrito> DistritoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DistritoRepository = DistritoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteDistritoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Distrito = await _DistritoRepository.GetByAsync(x => x.IdDistrito == request.Id);

            if (Distrito != null)
            {
                Distrito.Activo = false;
                await _DistritoRepository.UpdateAsync(Distrito);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
