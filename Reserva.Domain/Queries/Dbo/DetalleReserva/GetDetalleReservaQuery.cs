using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Domain.Queries.Dbo.DetalleReserva
{
    public class GetDetalleReservaQuery : QueryBase<GetDetalleReservaDto>
    {
        public GetDetalleReservaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
