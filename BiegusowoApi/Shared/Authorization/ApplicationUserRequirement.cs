using Microsoft.AspNetCore.Authorization;

namespace BiegusowoApi.Shared.Authorization;

public class ApplicationUserRequirement : IAuthorizationRequirement
{
    public ApplicationUserRequirement()
    {
    }
}
