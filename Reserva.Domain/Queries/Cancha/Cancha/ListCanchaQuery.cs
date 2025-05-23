using Reserva.Dto.Cancha.Cancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class ListCanchaQuery : QueryBase<IEnumerable<ListCanchaDto>>
    {
        public ListCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
