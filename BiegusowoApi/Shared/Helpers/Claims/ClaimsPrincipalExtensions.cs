using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiegusowoApi.Shared.Helpers.Claims;


public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!
                             ?? user.FindFirstValue("sub")!);

    }
}