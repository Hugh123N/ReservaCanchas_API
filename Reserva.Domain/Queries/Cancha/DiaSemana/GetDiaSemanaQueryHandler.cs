using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Cancha.DiaSemana
{
    public class GetDiaSemanaQueryHandler : QueryHandlerBase<GetDiaSemanaQuery, GetDiaSemanaDto>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public GetDiaSemanaQueryHandler(
            IMapper mapper,
            GetDiaSemanaQueryValidator validator,
            IRepository<Entity.Models.DiaSemana> DiaSemanaRepository
        ) : base(mapper, validator)
        {
            _DiaSemanaRepository = DiaSemanaRepository;
        }

        protected override async Task<ResponseDto<GetDiaSemanaDto>> HandleQuery(GetDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();
            var DiaSemana = await _DiaSemanaRepository.GetByAsync(x => x.IdDiaSemana == request.Id);
            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);

            if (DiaSemana != null && DiaSemanaDto != null)
            {
                response.UpdateData(DiaSemanaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
