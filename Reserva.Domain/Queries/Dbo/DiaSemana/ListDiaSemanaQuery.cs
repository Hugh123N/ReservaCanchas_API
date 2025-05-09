using Reserva.Dto.Dbo.DiaSemana;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.DiaSemana
{
    public class ListDiaSemanaQuery : QueryBase<IEnumerable<ListDiaSemanaDto>>
    {
        public ListDiaSemanaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
