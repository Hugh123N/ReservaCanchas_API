using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Queries.Cancha.GananciaProveedor
{
    public class GetGananciaProveedorQuery : QueryBase<GetGananciaProveedorDto>
    {
        public GetGananciaProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
