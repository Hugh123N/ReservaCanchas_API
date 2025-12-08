using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Hora;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class GetHoraQueryHandler : QueryHandlerBase<GetHoraQuery, GetHoraDto>
    {
        private readonly IRepository<Entity.Hora> _HoraRepository;

        public GetHoraQueryHandler(
            IMapper mapper,
            GetHoraQueryValidator validator,
            IRepository<Entity.Hora> HoraRepository
        ) : base(mapper, validator)
        {
            _HoraRepository = HoraRepository;
        }

        protected override async Task<ResponseDto<GetHoraDto>> HandleQuery(GetHoraQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetHoraDto>();
            var Hora = await _HoraRepository.GetByAsync(x => x.IdHora == request.Id);
            var HoraDto = _mapper?.Map<GetHoraDto>(Hora);

            if (Hora != null && HoraDto != null)
            {
                response.UpdateData(HoraDto);
            }

            return await Task.FromResult(response);
        }
    }
}
