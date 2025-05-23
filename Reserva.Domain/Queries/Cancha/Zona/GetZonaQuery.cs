using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class GetZonaQuery : QueryBase<GetZonaDto>
    {
        public GetZonaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
