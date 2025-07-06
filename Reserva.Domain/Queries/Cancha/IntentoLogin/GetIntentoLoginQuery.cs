using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Queries.Cancha.IntentoLogin
{
    public class GetIntentoLoginQuery : QueryBase<GetIntentoLoginDto>
    {
        public GetIntentoLoginQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
