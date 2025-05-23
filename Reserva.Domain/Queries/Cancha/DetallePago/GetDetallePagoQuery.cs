using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Domain.Queries.Cancha.DetallePago
{
    public class GetDetallePagoQuery : QueryBase<GetDetallePagoDto>
    {
        public GetDetallePagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
