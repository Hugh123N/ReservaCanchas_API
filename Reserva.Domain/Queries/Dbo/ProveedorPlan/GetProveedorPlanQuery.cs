using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetProveedorPlanQuery : QueryBase<GetProveedorPlanDto>
    {
        public GetProveedorPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
