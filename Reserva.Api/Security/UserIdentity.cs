using Reserva.Common;
using Reserva.Repository.Security;
using System.Security.Claims;

namespace Reserva.Api.Security
{
    public class UserIdentity : IUserIdentity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentity(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public IEnumerable<Claim> GetCurrentUserClaims()
            => _httpContextAccessor.HttpContext?.User?.Claims ?? [];

        public string GetIdSucursal()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "IdSucursal")?.Value!;

        private string GetUserNameClaim()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "UserName")?.Value!;

        public string GetUserName()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "DisplayName")?.Value!;

        public string GetUserRole()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value!;

        public string GetCurrentUser()
            => GetUserNameClaim() ?? Constants.Security.User.Admin;

        public int? GetCurrentUserId()
        {
            var userId = default(int?);
            var userIdClaim = GetCurrentUserClaims().FirstOrDefault(x => x.Type == "UserId")?.Value;
            if (userIdClaim != null) userId = int.Parse(userIdClaim);
            return userId;
        }
    }
}
