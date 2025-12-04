using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.HorarioCancha
{
    public class ListHorarioCanchaQuery : QueryBase<IEnumerable<ListHorarioCanchaDto>>
    {
        public ListHorarioCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
