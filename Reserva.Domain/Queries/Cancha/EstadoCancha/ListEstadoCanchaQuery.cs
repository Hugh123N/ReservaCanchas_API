using Reserva.Dto.Cancha.EstadoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class ListEstadoCanchaQuery : QueryBase<IEnumerable<ListEstadoCanchaDto>>
    {
        public ListEstadoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
