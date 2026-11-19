using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Queries.Dbo.Cancha
{
    public class GetCanchaQuery : QueryBase<GetCanchaDto>
    {
        public GetCanchaQuery(int id, bool gestion = false) { Id = id; Gestion = gestion; }
        public int Id { get; set; }
        public bool Gestion { get; }
    }
}
