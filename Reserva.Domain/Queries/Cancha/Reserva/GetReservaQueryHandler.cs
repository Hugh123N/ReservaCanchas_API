using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Reserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class GetReservaQueryHandler : QueryHandlerBase<GetReservaQuery, GetReservaDto>
    {
        private readonly IRepository<Entity.Models.Reserva> _ReservaRepository;

        public GetReservaQueryHandler(
            IMapper mapper,
            GetReservaQueryValidator validator,
            IRepository<Entity.Models.Reserva> ReservaRepository
        ) : base(mapper, validator)
        {
            _ReservaRepository = ReservaRepository;
        }

        protected override async Task<ResponseDto<GetReservaDto>> HandleQuery(GetReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();
            var Reserva = await _ReservaRepository.GetByAsync(x => x.IdReserva == request.Id);
            var ReservaDto = _mapper?.Map<GetReservaDto>(Reserva);

            if (Reserva != null && ReservaDto != null)
            {
                response.UpdateData(ReservaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
