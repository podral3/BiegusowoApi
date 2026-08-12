using BiegusowoApi.Data;
using BiegusowoApi.Shared.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiegusowoApi.Shared.Authorization;

public sealed class ApplicationUserAuthorizationHandler
: AuthorizationHandler<ApplicationUserRequirement>
{
    private readonly ApplicationDbContext _db;

    public ApplicationUserAuthorizationHandler(ApplicationDbContext db) => _db = db;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApplicationUserRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var keycloakUserId = context.User.GetUserId();

        var exists = await _db.Users
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == keycloakUserId,
                context.Resource is HttpContext httpContext
                    ? httpContext.RequestAborted
                    : CancellationToken.None);

        if (exists)
        {
            context.Succeed(requirement);
        }
    }
}
