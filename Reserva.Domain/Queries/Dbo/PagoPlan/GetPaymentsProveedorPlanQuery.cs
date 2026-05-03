using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class GetPaymentsProveedorPlanQuery : QueryBase<List<GetPagoPlanDto>>
    {
        public GetPaymentsProveedorPlanQuery(int idProveedor) => IdProveedor = idProveedor;
        public int IdProveedor { get; set; }
    }
}