using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class DeleteImagenCanchaCommandHandler : CommandHandlerBase<DeleteImagenCanchaCommand>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public DeleteImagenCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteImagenCanchaCommandValidator validator,
            IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteImagenCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var ImagenCancha = await _ImagenCanchaRepository.GetByAsync(x => x.IdImagenCancha == request.Id);

            if (ImagenCancha != null)
            {
                ImagenCancha.Activo = false;
                await _ImagenCanchaRepository.UpdateAsync(ImagenCancha);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
