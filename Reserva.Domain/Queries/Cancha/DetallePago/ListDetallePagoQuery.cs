using Reserva.Dto.Cancha.DetallePago;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class ListDetallePagoQuery : QueryBase<IEnumerable<ListDetallePagoDto>>
    {
        public ListDetallePagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
