using Reserva.Dto.Cancha.EstadoReserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class ListEstadoReservaQuery : QueryBase<IEnumerable<ListEstadoReservaDto>>
    {
        public ListEstadoReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
