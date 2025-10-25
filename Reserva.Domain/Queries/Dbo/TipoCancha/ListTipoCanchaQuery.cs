using Reserva.Dto.Dbo.TipoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.TipoCancha
{
    public class ListTipoCanchaQuery : QueryBase<IEnumerable<ListTipoCanchaDto>>
    {
        public ListTipoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
