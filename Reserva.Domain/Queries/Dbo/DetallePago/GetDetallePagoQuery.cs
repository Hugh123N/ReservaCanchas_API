using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Domain.Queries.Dbo.DetallePago
{
    public class GetDetallePagoQuery : QueryBase<GetDetallePagoDto>
    {
        public GetDetallePagoQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
