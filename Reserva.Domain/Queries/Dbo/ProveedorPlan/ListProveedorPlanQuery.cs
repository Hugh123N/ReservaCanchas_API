using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class ListProveedorPlanQuery : QueryBase<IEnumerable<ListProveedorPlanDto>>
    {
        public ListProveedorPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
