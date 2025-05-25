using Reserva.Dto.Cancha.TipoCancha;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.TipoCancha
{
    public class ListTipoCanchaQuery : QueryBase<IEnumerable<ListTipoCanchaDto>>
    {
        public ListTipoCanchaQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
