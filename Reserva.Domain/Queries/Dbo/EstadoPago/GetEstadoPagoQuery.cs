using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Domain.Queries.Dbo.EstadoPago
{
    public class GetEstadoPagoQuery : QueryBase<GetEstadoPagoDto>
    {
        public GetEstadoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
