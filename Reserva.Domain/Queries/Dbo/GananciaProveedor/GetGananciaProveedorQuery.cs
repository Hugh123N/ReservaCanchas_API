using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.GananciaProveedor;

namespace Reserva.Domain.Queries.Dbo.GananciaProveedor
{
    public class GetGananciaProveedorQuery : QueryBase<GetGananciaProveedorDto>
    {
        public GetGananciaProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
