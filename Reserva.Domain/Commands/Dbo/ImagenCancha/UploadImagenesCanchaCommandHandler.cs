using AutoMapper;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Storage;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ImagenCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.ImagenCancha
{
    public class UploadImagenesCanchaCommandHandler : CommandHandlerBase<UploadImagenesCanchaCommand, List<ImagenCanchaDto>>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.ImagenCancha> _imagenCanchaRepository;
        private readonly IStorageService _storageService;

        public UploadImagenesCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UploadImagenesCanchaCommandValidator validator,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.ImagenCancha> imagenCanchaRepository,
            IStorageService storageService
        ) : base(unitOfWork, mapper, validator)
        {
            _canchaRepository = canchaRepository;
            _imagenCanchaRepository = imagenCanchaRepository;
            _storageService = storageService;
        }

        public override async Task<ResponseDto<List<ImagenCanchaDto>>> HandleCommand(
            UploadImagenesCanchaCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<ImagenCanchaDto>>();

            var cancha = await _canchaRepository.GetByAsync(x => x.IdCancha == request.IdCancha);

            var imagenesCreadas = new List<Entity.ImagenCancha>();

            var existenImagenes = await _imagenCanchaRepository
                .FindByAsync(x => x.IdCancha == request.IdCancha && x.Activo);
            bool esPrimeraImagen = !existenImagenes.Any();

            // SI se especifica un índice principal >= 0, resetear todas las imágenes existentes en caso de update
            if (request.IndicePrincipal.HasValue && request.IndicePrincipal.Value >= 0 && existenImagenes.Any())
            {
                foreach (var img in existenImagenes)
                {
                    img.EsPrincipal = false;
                }
                await _imagenCanchaRepository.UpdateAsync(existenImagenes.ToArray());
                await _imagenCanchaRepository.SaveAsync();
            }

            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                // Generar path único: canchas/{idCancha}/imagen-{guid}.{ext}
                var extension = Path.GetExtension(file.FileName);
                var path = $"canchas/{request.IdCancha}/imagen-{Guid.NewGuid()}{extension}";

                var urlPublica = await _storageService.UploadAsync(
                    file.OpenReadStream(),
                    path,
                    file.ContentType
                );

                bool esPrincipal;
                if (request.IndicePrincipal.HasValue)
                    esPrincipal = i == request.IndicePrincipal.Value;
                else
                    esPrincipal = esPrimeraImagen && i == 0;

                var imagenCancha = new Entity.ImagenCancha
                {
                    IdCancha = request.IdCancha,
                    UrlImagen = urlPublica,
                    EsPrincipal = esPrincipal
                };

                imagenesCreadas.Add(imagenCancha);
            }
            await _imagenCanchaRepository.AddAsync(imagenesCreadas.ToArray());
            await _imagenCanchaRepository.SaveAsync();

            var imagenesDto = _mapper!.Map<List<ImagenCanchaDto>>(imagenesCreadas);
            response.UpdateData(imagenesDto);
            response.AddOkResult($"{imagenesCreadas.Count} imagen(es) subida(s) exitosamente");

            return response;
        }
    }
}
