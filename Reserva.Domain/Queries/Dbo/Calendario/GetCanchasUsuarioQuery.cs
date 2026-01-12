using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetCanchasUsuarioQuery : QueryBase<List<CanchaUsuarioDto>>
    {
        public List<string> Roles { get; set; } = new();
    }
}
