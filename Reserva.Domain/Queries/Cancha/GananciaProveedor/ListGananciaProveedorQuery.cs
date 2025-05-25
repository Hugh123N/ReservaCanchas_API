using Reserva.Dto.Cancha.GananciaProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class ListGananciaProveedorQuery : QueryBase<IEnumerable<ListGananciaProveedorDto>>
    {
        public ListGananciaProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
