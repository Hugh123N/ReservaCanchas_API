using Reserva.Dto.Dbo.Pago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Pago
{
    public class ListPagoQuery : QueryBase<IEnumerable<ListPagoDto>>
    {
        public ListPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
