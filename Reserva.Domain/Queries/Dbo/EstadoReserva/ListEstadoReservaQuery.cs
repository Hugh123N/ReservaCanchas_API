using Reserva.Dto.Dbo.EstadoReserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoReserva
{
    public class ListEstadoReservaQuery : QueryBase<IEnumerable<ListEstadoReservaDto>>
    {
        public ListEstadoReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
