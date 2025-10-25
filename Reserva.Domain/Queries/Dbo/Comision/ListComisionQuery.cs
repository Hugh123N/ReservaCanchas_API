using Reserva.Dto.Dbo.Comision;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Comision
{
    public class ListComisionQuery : QueryBase<IEnumerable<ListComisionDto>>
    {
        public ListComisionQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
