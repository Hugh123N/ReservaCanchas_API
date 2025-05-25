using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Queries.Cancha.EstadoProveedor
{
    public class GetEstadoProveedorQuery : QueryBase<GetEstadoProveedorDto>
    {
        public GetEstadoProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
