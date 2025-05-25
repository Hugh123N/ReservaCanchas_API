using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class DeleteUbigeoCommandHandler : CommandHandlerBase<DeleteUbigeoCommand>
    {
        private readonly IRepository<Entity.Models.Ubigeo> _UbigeoRepository;

        public DeleteUbigeoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteUbigeoCommandValidator validator,
            IRepository<Entity.Models.Ubigeo> UbigeoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _UbigeoRepository = UbigeoRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteUbigeoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Ubigeo = await _UbigeoRepository.GetByAsync(x => x.CodigoUbigeo == request.Id);

            if (Ubigeo != null)
            {
                Ubigeo.Activo = false;
                await _UbigeoRepository.UpdateAsync(Ubigeo);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
