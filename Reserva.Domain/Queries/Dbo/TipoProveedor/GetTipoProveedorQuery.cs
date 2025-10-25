using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.TipoProveedor;

namespace Reserva.Domain.Queries.Dbo.TipoProveedor
{
    public class GetTipoProveedorQuery : QueryBase<GetTipoProveedorDto>
    {
        public GetTipoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
