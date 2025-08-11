using AutoMapper;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class UpdateCanchaCommandHandler : CommandHandlerBase<UpdateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public UpdateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateCanchaCommandValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository,
            IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(UpdateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var imgsResultantes = new List<Entity.Models.ImagenCancha>();
            var Cancha = await _CanchaRepository.GetByAsNoTrackingAsync(x => x.IdCancha == request.UpdateDto.IdCancha);

            if (Cancha == null)
            {
                response.AddErrorResult("Cancha no encontrada.");
                return response;
            }

            var imgsBD = await _ImagenCanchaRepository.FindByAsNoTrackingAsync(x => x.IdCancha == request.UpdateDto.IdCancha) ?? new List<Entity.Models.ImagenCancha>();
           
            var idsRequest = request.UpdateDto.Imagenes?.Where(i => i.IdImagenCancha != 0).Select(i => i.IdImagenCancha)
                                               .ToList() ?? new List<int>();

            var imgsEliminar = imgsBD.Where(i => !idsRequest.Contains(i.IdImagenCancha)).ToList();
            foreach (var img in imgsEliminar)
            {
                img.Activo = false;
                imgsResultantes.Add(img);
            }

            var imgsActualizarDto = request.UpdateDto.Imagenes?.Where(i => i.IdImagenCancha != 0).ToList() ?? new List<UpdateImagenCanchaDto>();
            foreach (var dto in imgsActualizarDto)
            {
                var imgBD = imgsBD.FirstOrDefault(i => i.IdImagenCancha == dto.IdImagenCancha);
                if (imgBD != null)
                {
                    _mapper?.Map(dto, imgBD); 
                    imgsResultantes.Add(imgBD);
                }
            }

            var imgsNuevosDto = request.UpdateDto.Imagenes?.Where(x => x.IdImagenCancha == 0);

            var imgsNuevos = _mapper?.Map<List<Entity.Models.ImagenCancha>>(imgsNuevosDto) ?? new List<Entity.Models.ImagenCancha>();

            foreach (var img in imgsNuevos)
            {
                img.IdCancha = Cancha.IdCancha;
                img.UserNameCreate = Cancha.UserNameCreate;
                img.CreateDate = DateTimeOffset.Now;
                img.Activo = true;
                imgsResultantes.Add(img);
            }
            _mapper?.Map(request.UpdateDto, Cancha);
            Cancha.ImagenCanchas = imgsResultantes;

            await _CanchaRepository.UpdateAsync(Cancha);
            await _CanchaRepository.SaveAsync();
            
            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);
            return await Task.FromResult(response);
        }
    }
}
