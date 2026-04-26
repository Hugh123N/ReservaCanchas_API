using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Domain.Queries.Dbo.ComprobantePagoPlan
{
    public class GetComprobantePagoPlanQuery : QueryBase<GetComprobantePagoPlanDto>
    {
        public GetComprobantePagoPlanQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
