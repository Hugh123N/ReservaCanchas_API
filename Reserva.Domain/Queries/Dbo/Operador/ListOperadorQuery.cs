using Reserva.Dto.Dbo.Operador;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Operador
{
    public class ListOperadorQuery : QueryBase<IEnumerable<ListOperadorDto>>
    {
        public ListOperadorQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
