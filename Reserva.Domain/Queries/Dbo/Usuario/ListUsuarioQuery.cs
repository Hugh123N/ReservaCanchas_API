using Reserva.Dto.Dbo.Usuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class ListUsuarioQuery : QueryBase<IEnumerable<ListUsuarioDto>>
    {
        public ListUsuarioQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
