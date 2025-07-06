using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace NewsAggrigation.BLL.Services.Helper.UserIdentity
{
    public class UserIdentityContext : IUserIdentityContext
    {
        public int UserId { get; }

        public UserIdentityContext(IHttpContextAccessor accessor)
        {
            var userIdClaim = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                throw new ApiExceptionHelper("Invalid or missing user ID", 401);

            UserId = userId;
        }
    }
}
