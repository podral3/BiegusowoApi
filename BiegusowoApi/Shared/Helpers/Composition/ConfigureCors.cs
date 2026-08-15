namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureCors
{
    public static IServiceCollection ConfigureCorsOrigins(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                if (configuration.GetValue<bool>("IsDevelopment"))
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    policy
                        .WithOrigins("https://mapa-drobiu.pl")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });

        return services;
    }
}
