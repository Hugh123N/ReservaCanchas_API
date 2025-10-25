using Reserva.Dto.Dbo.IntentoLogin;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.IntentoLogin
{
    public class ListIntentoLoginQuery : QueryBase<IEnumerable<ListIntentoLoginDto>>
    {
        public ListIntentoLoginQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
