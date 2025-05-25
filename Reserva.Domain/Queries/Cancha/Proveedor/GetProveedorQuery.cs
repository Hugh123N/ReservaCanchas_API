using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Queries.Cancha.Proveedor
{
    public class GetProveedorQuery : QueryBase<GetProveedorDto>
    {
        public GetProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
