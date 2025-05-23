using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class GetZonaQueryHandler : QueryHandlerBase<GetZonaQuery, GetZonaDto>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public GetZonaQueryHandler(
            IMapper mapper,
            GetZonaQueryValidator validator,
            IRepository<Entity.Models.Zona> ZonaRepository
        ) : base(mapper, validator)
        {
            _ZonaRepository = ZonaRepository;
        }

        protected override async Task<ResponseDto<GetZonaDto>> HandleQuery(GetZonaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetZonaDto>();
            var Zona = await _ZonaRepository.GetByAsync(x => x.IdZona == request.Id);
            var ZonaDto = _mapper?.Map<GetZonaDto>(Zona);

            if (Zona != null && ZonaDto != null)
            {
                response.UpdateData(ZonaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
