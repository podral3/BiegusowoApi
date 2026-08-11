using Microsoft.AspNetCore.RateLimiting;

namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureRateLimiting
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("email-actions", opt =>
            {
                opt.PermitLimit = 3;
                opt.Window = TimeSpan.FromMinutes(15);
            });
        });
        return services;
    }
}
