using Reserva.Dto.Dbo.TipoProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class ListTipoProveedorQuery : QueryBase<IEnumerable<ListTipoProveedorDto>>
    {
        public ListTipoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
