using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetCurrentProveedorPlanQuery : QueryBase<GetProveedorPlanCurrentDto>
    {
        public GetCurrentProveedorPlanQuery(int idProveedor) => IdProveedor = idProveedor;
        public int IdProveedor { get; set; }
    }
}