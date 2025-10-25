using Reserva.Dto.Dbo.Reserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class ListReservaQuery : QueryBase<IEnumerable<ListReservaDto>>
    {
        public ListReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
