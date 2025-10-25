using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Queries.Dbo.Pago
{
    public class GetPagoQuery : QueryBase<GetPagoDto>
    {
        public GetPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
