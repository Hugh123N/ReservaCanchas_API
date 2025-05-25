using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Domain.Queries.Cancha.Reserva
{
    public class GetReservaQuery : QueryBase<GetReservaDto>
    {
        public GetReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
