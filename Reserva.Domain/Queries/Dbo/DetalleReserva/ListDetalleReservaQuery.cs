using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.DetalleReserva
{
    public class ListDetalleReservaQuery : QueryBase<IEnumerable<ListDetalleReservaDto>>
    {
        public ListDetalleReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
