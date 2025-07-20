using Reserva.Dto.Cancha.Proveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class ListProveedorQuery : QueryBase<IEnumerable<ListProveedorDto>>
    {
        public ListProveedorQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
