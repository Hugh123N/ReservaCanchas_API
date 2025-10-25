using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class GetDiaSemanaQueryHandler : QueryHandlerBase<GetDiaSemanaQuery, GetDiaSemanaDto>
    {
        private readonly IRepository<Entity.DiaSemana> _DiaSemanaRepository;

        public GetDiaSemanaQueryHandler(
            IMapper mapper,
            GetDiaSemanaQueryValidator validator,
            IRepository<Entity.DiaSemana> DiaSemanaRepository
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
