using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Queries.Cancha.EstadoCancha
{
    public class GetEstadoCanchaQuery : QueryBase<GetEstadoCanchaDto>
    {
        public GetEstadoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
