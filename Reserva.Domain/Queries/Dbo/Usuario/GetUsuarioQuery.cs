using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Usuario;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class GetUsuarioQuery : QueryBase<GetUsuarioDto>
    {
        public GetUsuarioQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
