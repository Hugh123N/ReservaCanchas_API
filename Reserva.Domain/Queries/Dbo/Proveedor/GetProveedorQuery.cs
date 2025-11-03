using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Proveedor;

namespace Reserva.Domain.Queries.Dbo.Proveedor
{
    public class GetProveedorQuery : QueryBase<GetProveedorDto>
    {
        public GetProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
