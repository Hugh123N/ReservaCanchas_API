using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Queries.Cancha.Pago
{
    public class GetPagoQuery : QueryBase<GetPagoDto>
    {
        public GetPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
