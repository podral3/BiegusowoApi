namespace BiegusowoApi.Helpers.Composition;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddKeycloakWebApiAuthentication(configuration);
        return services;
    }
}
