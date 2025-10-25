using Reserva.Dto.Dbo.Cancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class ListCanchaQuery : QueryBase<IEnumerable<ListCanchaDto>>
    {
        public ListCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
