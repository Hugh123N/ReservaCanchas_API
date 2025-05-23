using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Queries.Cancha.Cancha
{
    public class GetCanchaQuery : QueryBase<GetCanchaDto>
    {
        public GetCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
