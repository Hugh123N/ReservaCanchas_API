using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Queries.Cancha.EstadoReserva
{
    public class GetEstadoReservaQuery : QueryBase<GetEstadoReservaDto>
    {
        public GetEstadoReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
