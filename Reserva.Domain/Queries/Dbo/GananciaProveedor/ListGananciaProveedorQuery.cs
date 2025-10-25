using Reserva.Dto.Dbo.GananciaProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.GananciaProveedor
{
    public class ListGananciaProveedorQuery : QueryBase<IEnumerable<ListGananciaProveedorDto>>
    {
        public ListGananciaProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
