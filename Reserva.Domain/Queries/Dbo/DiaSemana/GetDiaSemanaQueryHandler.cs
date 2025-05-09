using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Domain.Queries.Dbo.DiaSemana;
using Reserva.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class GetDiaSemanaQueryHandler : QueryHandlerBase<GetDiaSemanaQuery, GetDiaSemanaDto>
    {
        private readonly ReservaCanchasContext _context;

        public GetDiaSemanaQueryHandler(
            IMapper mapper,
            GetDiaSemanaQueryValidator validator,
            ReservaCanchasContext context
        ) : base(mapper, validator)
        {
            _context = context;
        }

        protected override async Task<ResponseDto<GetDiaSemanaDto>> HandleQuery(GetDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();
            var DiaSemana = await _context.DiaSemanas.FirstOrDefaultAsync(x => x.IdDia == request.Id);
            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);

            if (DiaSemana != null && DiaSemanaDto != null)
            {
                response.UpdateData(DiaSemanaDto);
            }

            return await Task.FromResult(response);
        }
    }
}
