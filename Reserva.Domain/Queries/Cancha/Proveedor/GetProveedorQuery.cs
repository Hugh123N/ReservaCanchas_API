using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class GetProveedorQuery : QueryBase<GetProveedorDto>
    {
        public GetProveedorQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
