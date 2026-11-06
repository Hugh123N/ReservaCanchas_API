using Reserva.Domain.Queries.Base;

namespace Reserva.Domain.Queries.Dbo.Usuario
{
    public class GetUsuarioNegocioQuery : QueryBase<int?>
    {
        public GetUsuarioNegocioQuery(Guid idUsuario, string? applicationCode, IEnumerable<string> roles)
        {
            ArgumentNullException.ThrowIfNull(roles);
            IdUsuario = idUsuario;
            ApplicationCode = applicationCode?.Trim() ?? string.Empty;
            Roles = roles.ToArray();
        }

        public Guid IdUsuario { get; }
        public string ApplicationCode { get; }
        public IReadOnlyCollection<string> Roles { get; }
    }
}
