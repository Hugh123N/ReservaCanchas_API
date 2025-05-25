using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Domain.Queries.Cancha.TipoProveedor
{
    public class GetTipoProveedorQuery : QueryBase<GetTipoProveedorDto>
    {
        public GetTipoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
