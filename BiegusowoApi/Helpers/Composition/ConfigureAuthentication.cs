using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Common;
using Duende.AccessTokenManagement;

namespace BiegusowoApi.Helpers.Composition;

public static class ConfigureAuthentication
{
    private static readonly ClientCredentialsClientName KeycloakAdminClientName =
        ClientCredentialsClientName.Parse("keycloak-admin");
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure admin client for Keycloak Admin API access
        var keycloakAdminOptions = configuration
            .GetKeycloakOptions<KeycloakAdminClientOptions>("KeycloakAdmin")!;

        services.AddDistributedMemoryCache();

        services
            .AddClientCredentialsTokenManagement()
            .AddClient("keycloak-admin", client =>
            {
                client.ClientId = ClientId.Parse(keycloakAdminOptions.Resource);
                client.ClientSecret = ClientSecret.Parse(keycloakAdminOptions.Credentials.Secret);
                client.TokenEndpoint = new Uri(keycloakAdminOptions.KeycloakTokenEndpoint);
            });

        services
            .AddKeycloakAdminHttpClient(keycloakAdminOptions)
            .AddClientCredentialsTokenHandler(KeycloakAdminClientName);

        //Configure Keycloak authentication for the API for normal user auth flow
        services.AddKeycloakWebApiAuthentication(configuration);
        return services;
    }
}
