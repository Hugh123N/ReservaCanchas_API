using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.ImagenCancha
{
    public class UpdateImagenCanchaCommandHandler : CommandHandlerBase<UpdateImagenCanchaCommand, GetImagenCanchaDto>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public UpdateImagenCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateImagenCanchaCommandValidator validator,
            IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        public override async Task<ResponseDto<GetImagenCanchaDto>> HandleCommand(UpdateImagenCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetImagenCanchaDto>();
            var ImagenCancha = await _ImagenCanchaRepository.GetByAsync(x => x.IdImagenCancha == request.UpdateDto.IdImagenCancha);

            if (ImagenCancha != null)
            {
                _mapper?.Map(request.UpdateDto, ImagenCancha);
                await _ImagenCanchaRepository.UpdateAsync(ImagenCancha);
                await _ImagenCanchaRepository.SaveAsync();
            }

            var ImagenCanchaDto = _mapper?.Map<GetImagenCanchaDto>(ImagenCancha);
            if (ImagenCanchaDto != null) response.UpdateData(ImagenCanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
