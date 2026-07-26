using BiegusowoApi.Auth.CurrentUser;

namespace BiegusowoApi.Helpers.Composition;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {

        services.AddAuthentication()
         .AddKeycloakJwtBearer(
            serviceName: "keycloak", //configuration["Keycloak:ServiceName"]!,
            realm: "biegusowo", //configuration["Keycloak:Realm"]!,
            options =>
            {
                options.Audience = "biegusowoapi"; //configuration["Keycloak:Audience"];
                if (environment.IsDevelopment())
                {
                    options.RequireHttpsMetadata = false;
                }
            });

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
