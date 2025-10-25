using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoCancha;

namespace Reserva.Domain.Queries.Dbo.EstadoCancha
{
    public class GetEstadoCanchaQuery : QueryBase<GetEstadoCanchaDto>
    {
        public GetEstadoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
