using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class GetEstadoReservaQueryHandler : QueryHandlerBase<GetEstadoReservaQuery, GetEstadoReservaDto>
    {
        private readonly IRepository<Entity.Models.EstadoReserva> _EstadoReservaRepository;

        public GetEstadoReservaQueryHandler(
            IMapper mapper,
            GetEstadoReservaQueryValidator validator,
            IRepository<Entity.Models.EstadoReserva> EstadoReservaRepository
        ) : base(mapper, validator)
        {
            _EstadoReservaRepository = EstadoReservaRepository;
        }

        protected override async Task<ResponseDto<GetEstadoReservaDto>> HandleQuery(GetEstadoReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoReservaDto>();
            var EstadoReserva = await _EstadoReservaRepository.GetByAsync(x => x.IdEstadoReserva == request.Id);
            var EstadoReservaDto = _mapper?.Map<GetEstadoReservaDto>(EstadoReserva);

            if (EstadoReserva != null && EstadoReservaDto != null)
            {
                response.UpdateData(EstadoReservaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
