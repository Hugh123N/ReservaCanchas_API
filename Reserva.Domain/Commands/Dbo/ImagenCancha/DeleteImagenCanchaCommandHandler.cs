using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Storage;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    /// <summary>
    /// Handler para eliminar una imagen de cancha (soft delete en BD + eliminación física en storage)
    /// </summary>
    public class DeleteImagenCanchaCommandHandler : CommandHandlerBase<DeleteImagenCanchaCommand>
    {
        private readonly IRepository<Entity.ImagenCancha> _ImagenCanchaRepository;
        private readonly IStorageService _storageService;

        public DeleteImagenCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeleteImagenCanchaCommandValidator validator,
            IRepository<Entity.ImagenCancha> ImagenCanchaRepository,
            IStorageService storageService
        ) : base(unitOfWork, mapper, validator)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
            _storageService = storageService;
        }

        public override async Task<ResponseDto> HandleCommand(DeleteImagenCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var ImagenCancha = await _ImagenCanchaRepository.GetByAsync(x => x.IdImagenCancha == request.Id);

            var uri = new Uri(ImagenCancha.UrlImagen);
            var path = uri.AbsolutePath.TrimStart('/');

            // Eliminar de Cloudflare R2
            await _storageService.DeleteAsync(path);

            await _ImagenCanchaRepository.DeleteAsync(ImagenCancha);

            response.AddOkResult(Resources.Common.DeleteSuccessMessage);

            return response;
        }
    }
}
