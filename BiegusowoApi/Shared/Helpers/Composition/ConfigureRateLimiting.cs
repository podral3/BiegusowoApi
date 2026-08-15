using BiegusowoApi.Shared.Helpers.Claims;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace BiegusowoApi.Shared.Helpers.Composition;
public static class ConfigureRateLimiting
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("presigned-uploads", context =>
                {
                    var userId = context.User.GetUserId().ToString();

                    var partitionKey = !string.IsNullOrEmpty(userId)
                        ? $"user:{userId}"
                        : $"ip:{context.Connection.RemoteIpAddress}";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(15),
                            QueueLimit = 0
                        });
                });
            });
        });
        return services;
    }
}
