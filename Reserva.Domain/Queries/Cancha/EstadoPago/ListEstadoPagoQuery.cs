using Reserva.Dto.Cancha.EstadoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class ListEstadoPagoQuery : QueryBase<IEnumerable<ListEstadoPagoDto>>
    {
        public ListEstadoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
