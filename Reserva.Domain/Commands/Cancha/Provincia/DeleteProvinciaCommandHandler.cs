using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class DeleteProvinciaCommandHandler : CommandHandlerBase<DeleteProvinciaCommand>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public DeleteProvinciaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteProvinciaCommandValidator validator,
            IRepository<Entity.Models.Provincia> ProvinciaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProvinciaRepository = ProvinciaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteProvinciaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Provincia = await _ProvinciaRepository.GetByAsync(x => x.IdProvincia == request.Id);

            if (Provincia != null)
            {
                Provincia.Activo = false;
                await _ProvinciaRepository.UpdateAsync(Provincia);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
