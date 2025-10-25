using Reserva.Dto.Dbo.EstadoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoPago
{
    public class ListEstadoPagoQuery : QueryBase<IEnumerable<ListEstadoPagoDto>>
    {
        public ListEstadoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
