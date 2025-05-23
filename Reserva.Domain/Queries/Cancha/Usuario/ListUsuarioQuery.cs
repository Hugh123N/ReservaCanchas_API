using Reserva.Dto.Cancha.Usuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class ListUsuarioQuery : QueryBase<IEnumerable<ListUsuarioDto>>
    {
        public ListUsuarioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
