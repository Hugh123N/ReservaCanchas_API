using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class GetReservaQuery : QueryBase<GetReservaDto>
    {
        public GetReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
