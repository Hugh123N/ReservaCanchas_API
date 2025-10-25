using Reserva.Dto.Dbo.EstadoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class ListEstadoProveedorQuery : QueryBase<IEnumerable<ListEstadoProveedorDto>>
    {
        public ListEstadoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
