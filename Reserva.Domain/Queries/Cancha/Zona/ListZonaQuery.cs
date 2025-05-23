using Reserva.Dto.Cancha.Zona;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Zona
{
    public class ListZonaQuery : QueryBase<IEnumerable<ListZonaDto>>
    {
        public ListZonaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
