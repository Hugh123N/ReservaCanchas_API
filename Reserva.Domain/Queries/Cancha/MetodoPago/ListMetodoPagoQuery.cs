using Reserva.Dto.Cancha.MetodoPago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class ListMetodoPagoQuery : QueryBase<IEnumerable<ListMetodoPagoDto>>
    {
        public ListMetodoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
