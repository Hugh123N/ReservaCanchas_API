using Reserva.Dto.Dbo.PlanLimite;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.PlanLimite
{
    public class ListPlanLimiteQuery : QueryBase<IEnumerable<ListPlanLimiteDto>>
    {
        public ListPlanLimiteQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
