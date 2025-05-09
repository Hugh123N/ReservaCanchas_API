using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Domain.Queries.Base;
using Reserva.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class ListDiaSemanaQueryHandler : QueryHandlerBase<ListDiaSemanaQuery, IEnumerable<ListDiaSemanaDto>>
    {
        private readonly ReservaCanchasContext _context;

        public ListDiaSemanaQueryHandler(
            IMapper mapper,
            ReservaCanchasContext context
        ) : base(mapper)
        {
            _context = context;
        }

        protected override async Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> HandleQuery(ListDiaSemanaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListDiaSemanaDto>>();
            var list = await _context.DiaSemanas.Where(x => x.IdDia == request.Id).AsNoTracking().ToListAsync();
            var listDtos = _mapper?.Map<IEnumerable<ListDiaSemanaDto>>(list);

            response.UpdateData(listDtos ?? new List<ListDiaSemanaDto>());

            return await Task.FromResult(response);
        }
    }
}
