using Reserva.Dto.Cancha.EstadoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class ListEstadoProveedorQuery : QueryBase<IEnumerable<ListEstadoProveedorDto>>
    {
        public ListEstadoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
