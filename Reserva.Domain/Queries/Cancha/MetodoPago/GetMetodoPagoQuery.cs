using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Queries.Cancha.MetodoPago
{
    public class GetMetodoPagoQuery : QueryBase<GetMetodoPagoDto>
    {
        public GetMetodoPagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
