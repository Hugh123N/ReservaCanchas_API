using Reserva.Dto.Cancha.Rol;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Rol
{
    public class ListRolQuery : QueryBase<IEnumerable<ListRolDto>>
    {
        public ListRolQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
