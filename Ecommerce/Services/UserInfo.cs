using System.Security.Claims;

namespace Ecommerce.Services
{
    public static class UserInfo
    {
        public static string GetUserId(ClaimsPrincipal User)
        {
            string userId = null;
            foreach (ClaimsIdentity? identity in User.Identities)
            {
                Claim? claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    userId = claim.Value;
                    break;
                }
            }
            return userId;
        }
    }
}
