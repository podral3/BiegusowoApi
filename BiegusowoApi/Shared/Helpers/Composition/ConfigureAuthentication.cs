using BiegusowoApi.Shared.Authorization;
using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authorization;

namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureAuthentication
{
    private static readonly ClientCredentialsClientName KeycloakAdminClientName =
        ClientCredentialsClientName.Parse("keycloak-admin");

    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ------------------------------------------------------------
        // Keycloak Admin API client
        // ------------------------------------------------------------

        var keycloakAdminOptions = configuration
            .GetKeycloakOptions<KeycloakAdminClientOptions>("KeycloakAdmin")!;

        services.AddDistributedMemoryCache();

        services
            .AddClientCredentialsTokenManagement()
            .AddClient("keycloak-admin", client =>
            {
                client.ClientId = ClientId.Parse(keycloakAdminOptions.Resource);
                client.ClientSecret =
                    ClientSecret.Parse(keycloakAdminOptions.Credentials.Secret);

                client.TokenEndpoint =
                    new Uri(keycloakAdminOptions.KeycloakTokenEndpoint);
            });

        services
            .AddKeycloakAdminHttpClient(keycloakAdminOptions)
            .AddClientCredentialsTokenHandler(KeycloakAdminClientName);

        // ------------------------------------------------------------
        // Normal user authentication
        // ------------------------------------------------------------

        services.AddKeycloakWebApiAuthentication(configuration);

        // ------------------------------------------------------------
        // Application authorization
        // ------------------------------------------------------------

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.ApplicationUser,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new ApplicationUserRequirement());
                });
        });

        services.AddScoped<IAuthorizationHandler, ApplicationUserAuthorizationHandler>();

        return services;
    }
}