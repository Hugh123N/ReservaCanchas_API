using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoReserva;

namespace Reserva.Domain.Queries.Dbo.EstadoReserva
{
    public class GetEstadoReservaQuery : QueryBase<GetEstadoReservaDto>
    {
        public GetEstadoReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
