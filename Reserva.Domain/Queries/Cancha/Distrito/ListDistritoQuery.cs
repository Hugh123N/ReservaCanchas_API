using Reserva.Dto.Cancha.Distrito;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Distrito
{
    public class ListDistritoQuery : QueryBase<IEnumerable<ListDistritoDto>>
    {
        public ListDistritoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
