using Reserva.Dto.Dbo.ConfiguracionProveedor;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.ConfiguracionProveedor
{
    public class ListConfiguracionProveedorQuery : QueryBase<IEnumerable<ListConfiguracionProveedorDto>>
    {
        public ListConfiguracionProveedorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
