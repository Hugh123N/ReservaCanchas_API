using Reserva.Dto.Dbo.EstadoUsuario;
using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.EstadoUsuario
{
    public class ListEstadoUsuarioQuery : QueryBase<IEnumerable<ListEstadoUsuarioDto>>
    {
        public ListEstadoUsuarioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
