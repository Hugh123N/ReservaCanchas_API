using Reserva.Dto.Dbo.Proveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class ListProveedorQuery : QueryBase<IEnumerable<ListProveedorDto>>
    {
        public ListProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
