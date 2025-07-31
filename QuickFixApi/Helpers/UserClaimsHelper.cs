using System.Security.Claims;

namespace QuickFixApi.Helpers
{
    public static class UserClaimsHelper
    {
        public static string GetEmail(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
        }
    }
}
