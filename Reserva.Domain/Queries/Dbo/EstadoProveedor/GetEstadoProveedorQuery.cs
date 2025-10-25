using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoProveedor;

namespace Reserva.Domain.Queries.Dbo.EstadoProveedor
{
    public class GetEstadoProveedorQuery : QueryBase<GetEstadoProveedorDto>
    {
        public GetEstadoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
