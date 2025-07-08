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

        public string? GetCurrentToken()
            => _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        private string GetUserNameClaim()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "UserName")?.Value!;

        public string GetUserName()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == "DisplayName")?.Value!;

        public string GetUserRole()
            => GetCurrentUserClaims()?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value!;

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
