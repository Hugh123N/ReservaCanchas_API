using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class GetConfiguracionProveedorQuery : QueryBase<GetConfiguracionProveedorDto>
    {
        public GetConfiguracionProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
