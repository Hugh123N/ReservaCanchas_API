using Reserva.Dto.Cancha.IntentoLogin;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class ListIntentoLoginQuery : QueryBase<IEnumerable<ListIntentoLoginDto>>
    {
        public ListIntentoLoginQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
