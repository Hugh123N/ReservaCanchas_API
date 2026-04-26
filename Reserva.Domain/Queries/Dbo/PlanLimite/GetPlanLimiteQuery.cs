using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Domain.Queries.Dbo.PlanLimite
{
    public class GetPlanLimiteQuery : QueryBase<GetPlanLimiteDto>
    {
        public GetPlanLimiteQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
