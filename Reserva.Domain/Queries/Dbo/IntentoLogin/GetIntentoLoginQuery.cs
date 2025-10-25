using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.IntentoLogin;

namespace Reserva.Domain.Queries.Dbo.IntentoLogin
{
    public class GetIntentoLoginQuery : QueryBase<GetIntentoLoginDto>
    {
        public GetIntentoLoginQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
