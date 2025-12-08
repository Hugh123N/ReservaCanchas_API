using Reserva.Dto.Dbo.Hora;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Hora
{
    public class ListHoraQuery : QueryBase<IEnumerable<ListHoraDto>>
    {
        public ListHoraQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
