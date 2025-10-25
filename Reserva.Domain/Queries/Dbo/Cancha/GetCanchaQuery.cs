using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class GetCanchaQuery : QueryBase<GetCanchaDto>
    {
        public GetCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
