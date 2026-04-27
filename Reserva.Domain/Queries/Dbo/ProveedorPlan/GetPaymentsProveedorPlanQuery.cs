using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetPaymentsProveedorPlanQuery : QueryBase<List<PagoPlanDto>>
    {
        public GetPaymentsProveedorPlanQuery(int idProveedor) => IdProveedor = idProveedor;
        public int IdProveedor { get; set; }
    }
}