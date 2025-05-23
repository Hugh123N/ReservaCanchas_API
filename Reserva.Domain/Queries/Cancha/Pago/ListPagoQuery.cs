using Reserva.Dto.Cancha.Pago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class ListPagoQuery : QueryBase<IEnumerable<ListPagoDto>>
    {
        public ListPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
