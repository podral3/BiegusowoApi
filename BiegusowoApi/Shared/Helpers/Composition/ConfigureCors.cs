namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureCors
{
    public static IServiceCollection ConfigureCorsOrigins(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowFrontend", policy =>
                policy.WithOrigins("https://mapa-drobiu.pl")
                    .AllowAnyMethod()
                    .AllowAnyHeader()));
        return services;
    }
}
