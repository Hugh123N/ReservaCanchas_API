using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Servicio
{
    public class GetServicioQueryHandler : QueryHandlerBase<GetServicioQuery, GetServicioDto>
    {
        private readonly IRepository<Entity.Servicio> _ServicioRepository;

        public GetServicioQueryHandler(
            IMapper mapper,
            GetServicioQueryValidator validator,
            IRepository<Entity.Servicio> ServicioRepository
        ) : base(mapper, validator)
        {
            _ServicioRepository = ServicioRepository;
        }

        protected override async Task<ResponseDto<GetServicioDto>> HandleQuery(GetServicioQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetServicioDto>();
            var Servicio = await _ServicioRepository.GetByAsync(x => x.IdServicio == request.Id);
            var ServicioDto = _mapper?.Map<GetServicioDto>(Servicio);

            if (Servicio != null && ServicioDto != null)
            {
                response.UpdateData(ServicioDto);
            }

            return await Task.FromResult(response);
        }
    }
}
