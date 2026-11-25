using Reserva.Repository.Security;
using System.Security.Claims;

namespace Reserva.Domain.Services.Reclamaciones
{
    public static class ReclamacionReglas
    {
        public static bool EsAdmin(IUserIdentity identity) => identity.GetCurrentUserClaims().Any(c =>
            (c.Type == "Roles" || c.Type == ClaimTypes.Role) && c.Value == Reserva.Common.Constants.Role.Admin);

        public static string? Opcional(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
