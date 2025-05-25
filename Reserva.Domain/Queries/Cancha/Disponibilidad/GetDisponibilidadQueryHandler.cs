using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Disponibilidad;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Disponibilidad
{
    public class GetDisponibilidadQueryHandler : QueryHandlerBase<GetDisponibilidadQuery, GetDisponibilidadDto>
    {
        private readonly IRepository<Entity.Models.Disponibilidad> _DisponibilidadRepository;

        public GetDisponibilidadQueryHandler(
            IMapper mapper,
            GetDisponibilidadQueryValidator validator,
            IRepository<Entity.Models.Disponibilidad> DisponibilidadRepository
        ) : base(mapper, validator)
        {
            _DisponibilidadRepository = DisponibilidadRepository;
        }

        protected override async Task<ResponseDto<GetDisponibilidadDto>> HandleQuery(GetDisponibilidadQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDisponibilidadDto>();
            var Disponibilidad = await _DisponibilidadRepository.GetByAsync(x => x.IdDisponibilidad == request.Id);
            var DisponibilidadDto = _mapper?.Map<GetDisponibilidadDto>(Disponibilidad);

            if (Disponibilidad != null && DisponibilidadDto != null)
            {
                response.UpdateData(DisponibilidadDto);
            }

            return await Task.FromResult(response);
        }
    }
}
