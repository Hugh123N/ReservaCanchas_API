using Reserva.Dto.Dbo.EstadoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoCancha
{
    public class ListEstadoCanchaQuery : QueryBase<IEnumerable<ListEstadoCanchaDto>>
    {
        public ListEstadoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
