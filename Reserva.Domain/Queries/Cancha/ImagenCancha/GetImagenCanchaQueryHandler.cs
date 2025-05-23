using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.ImagenCancha;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.ImagenCancha
{
    public class GetImagenCanchaQueryHandler : QueryHandlerBase<GetImagenCanchaQuery, GetImagenCanchaDto>
    {
        private readonly IRepository<Entity.Models.ImagenCancha> _ImagenCanchaRepository;

        public GetImagenCanchaQueryHandler(
            IMapper mapper,
            GetImagenCanchaQueryValidator validator,
            IRepository<Entity.Models.ImagenCancha> ImagenCanchaRepository
        ) : base(mapper, validator)
        {
            _ImagenCanchaRepository = ImagenCanchaRepository;
        }

        protected override async Task<ResponseDto<GetImagenCanchaDto>> HandleQuery(GetImagenCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetImagenCanchaDto>();
            var ImagenCancha = await _ImagenCanchaRepository.GetByAsync(x => x.IdImagenCancha == request.Id);
            var ImagenCanchaDto = _mapper?.Map<GetImagenCanchaDto>(ImagenCancha);

            if (ImagenCancha != null && ImagenCanchaDto != null)
            {
                response.UpdateData(ImagenCanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
