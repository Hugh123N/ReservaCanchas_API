using Reserva.Dto.Cancha.Usuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class ListUsuarioQuery : QueryBase<IEnumerable<ListUsuarioDto>>
    {
        public ListUsuarioQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
