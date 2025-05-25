using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Domain.Queries.Cancha.EstadoPago
{
    public class GetEstadoPagoQuery : QueryBase<GetEstadoPagoDto>
    {
        public GetEstadoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
