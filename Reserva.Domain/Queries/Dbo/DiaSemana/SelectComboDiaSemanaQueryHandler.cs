using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Domain.Queries.Base;
using Reserva.Entity.Models;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class SelectComboDiaSemanaQueryHandler : QueryHandlerBase<SelectComboDiaSemanaQuery, IEnumerable<SelectComboDiaSemanaDto>>
    {
        private readonly ReservaCanchasContext _context;

        public SelectComboDiaSemanaQueryHandler(
            IMapper mapper,
            ReservaCanchasContext context
        ) : base(mapper)
        {
            _context = context;
        }

        protected override async Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> HandleQuery(SelectComboDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>();
            var list = await _context.DiaSemanas.ToListAsync();
            var listDtos = _mapper?.Map<IEnumerable<SelectComboDiaSemanaDto>>(list);

            response.UpdateData(listDtos ?? new List<SelectComboDiaSemanaDto>());

            return await Task.FromResult(response);
        }
    }
}
