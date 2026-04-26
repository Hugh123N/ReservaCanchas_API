using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class ListPagoPlanQuery : QueryBase<IEnumerable<ListPagoPlanDto>>
    {
        public ListPagoPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
