using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class GetRolQuery : QueryBase<GetRolDto>
    {
        public GetRolQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
