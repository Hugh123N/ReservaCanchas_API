using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.EstadoUsuario;

namespace Reserva.Domain.Queries.Dbo.EstadoUsuario
{
    public class GetEstadoUsuarioQuery : QueryBase<GetEstadoUsuarioDto>
    {
        public GetEstadoUsuarioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
