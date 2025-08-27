using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class GetUsuarioQuery : QueryBase<GetUsuarioDto>
    {
        public GetUsuarioQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
