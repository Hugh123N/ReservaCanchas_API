using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class GetCanchaConfigQuery : QueryBase<GetCanchaConfigDto>
    {
        public GetCanchaConfigQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
