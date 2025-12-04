using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class GetHorarioCanchaQueryHandler : QueryHandlerBase<GetHorarioCanchaQuery, GetHorarioCanchaDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public GetHorarioCanchaQueryHandler(
            IMapper mapper,
            GetHorarioCanchaQueryValidator validator,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository
        ) : base(mapper, validator)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
        }

        protected override async Task<ResponseDto<GetHorarioCanchaDto>> HandleQuery(GetHorarioCanchaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetHorarioCanchaDto>();
            var HorarioCancha = await _HorarioCanchaRepository.GetByAsync(x => x.IdHorarioCancha == request.Id);
            var HorarioCanchaDto = _mapper?.Map<GetHorarioCanchaDto>(HorarioCancha);

            if (HorarioCancha != null && HorarioCanchaDto != null)
            {
                response.UpdateData(HorarioCanchaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
