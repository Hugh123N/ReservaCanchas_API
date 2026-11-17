using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class GetReservaQuery : QueryBase<GetReservaDto>
    {
        public GetReservaQuery(int id, bool gestion = false) { Id = id; Gestion = gestion; }
        public int Id { get; set; }
        public bool Gestion { get; }
    }
}
