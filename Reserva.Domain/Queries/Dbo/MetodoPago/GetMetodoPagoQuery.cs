using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.MetodoPago;

namespace Reserva.Domain.Queries.Dbo.MetodoPago
{
    public class GetMetodoPagoQuery : QueryBase<GetMetodoPagoDto>
    {
        public GetMetodoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
