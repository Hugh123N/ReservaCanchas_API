using Reserva.Dto.Cancha.Comision;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Comision
{
    public class ListComisionQuery : QueryBase<IEnumerable<ListComisionDto>>
    {
        public ListComisionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
