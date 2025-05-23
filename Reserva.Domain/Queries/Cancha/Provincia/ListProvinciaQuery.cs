using Reserva.Dto.Cancha.Provincia;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Provincia
{
    public class ListProvinciaQuery : QueryBase<IEnumerable<ListProvinciaDto>>
    {
        public ListProvinciaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
