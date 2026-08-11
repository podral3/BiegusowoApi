using System.Security.Claims;

namespace BiegusowoApi.Shared.Helpers.Claims;


public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}