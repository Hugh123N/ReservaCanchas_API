using Reserva.Dto.Dbo.ComprobantePagoPlan;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class ListComprobantePagoPlanQuery : QueryBase<IEnumerable<ListComprobantePagoPlanDto>>
    {
        public ListComprobantePagoPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
