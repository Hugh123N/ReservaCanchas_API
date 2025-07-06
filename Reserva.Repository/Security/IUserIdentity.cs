using System.Security.Claims;

namespace Reserva.Repository.Security
{
    public interface IUserIdentity
    {
        IEnumerable<Claim> GetCurrentUserClaims();
        //string GetIdSucursal();
        string GetCurrentUser();
        string GetUserName();
        string GetUserRole();
        int? GetCurrentUserId();
    }
}
