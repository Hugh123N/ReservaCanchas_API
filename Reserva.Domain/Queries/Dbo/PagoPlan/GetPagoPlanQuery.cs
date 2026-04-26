using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class GetPagoPlanQuery : QueryBase<GetPagoPlanDto>
    {
        public GetPagoPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
