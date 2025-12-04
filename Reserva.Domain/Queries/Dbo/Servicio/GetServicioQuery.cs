using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Domain.Queries.Dbo.Servicio
{
    public class GetServicioQuery : QueryBase<GetServicioDto>
    {
        public GetServicioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
