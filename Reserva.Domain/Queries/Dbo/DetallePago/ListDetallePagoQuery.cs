using Reserva.Dto.Dbo.DetallePago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.DetallePago
{
    public class ListDetallePagoQuery : QueryBase<IEnumerable<ListDetallePagoDto>>
    {
        public ListDetallePagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
