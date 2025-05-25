using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Queries.Cancha.EstadoUsuario
{
    public class GetEstadoUsuarioQuery : QueryBase<GetEstadoUsuarioDto>
    {
        public GetEstadoUsuarioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
