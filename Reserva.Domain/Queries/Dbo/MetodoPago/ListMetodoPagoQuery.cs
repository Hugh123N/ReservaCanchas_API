using Reserva.Dto.Dbo.MetodoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.MetodoPago
{
    public class ListMetodoPagoQuery : QueryBase<IEnumerable<ListMetodoPagoDto>>
    {
        public ListMetodoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
