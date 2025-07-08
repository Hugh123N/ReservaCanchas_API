using System.Security.Claims;

namespace Reserva.Repository.Security
{
    public interface IUserIdentity
    {
        IEnumerable<Claim> GetCurrentUserClaims();

        public string? GetCurrentToken();
        string GetCurrentUser();
        string GetUserName();
        string GetUserRole();
        int? GetCurrentUserId();
    }
}
