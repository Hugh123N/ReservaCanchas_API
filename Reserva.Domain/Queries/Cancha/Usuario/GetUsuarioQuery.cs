using Reserva.Domain.Queries.Base;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Queries.Cancha.Usuario
{
    public class GetUsuarioQuery : QueryBase<GetUsuarioDto>
    {
        public GetUsuarioQuery(int id) => Id = id;
        public int Id { get; set; }
    }
}
