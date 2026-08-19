using BiegusowoApi.Shared.Options;
using Duende.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureAuthentication
{
    private static readonly ClientCredentialsClientName KeycloakAdminClientName =
        ClientCredentialsClientName.Parse("keycloak-admin");

    public static IServiceCollection AddSupabaseAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var supabaseOptions = configuration
                .GetSection("Supabase").Get<SupabaseJwtOptions>()!;

            options.Authority = $"{supabaseOptions.Url}/auth/v1";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = supabaseOptions.ValidateIssuerSigningKey,
                ValidIssuer = supabaseOptions.ValidIssuer,
                ValidAudience = supabaseOptions.ValidAudience,
                ValidateIssuer = supabaseOptions.ValidateIssuer,
                ValidateAudience = supabaseOptions.ValidateAudience,
                ValidateLifetime = supabaseOptions.ValidateLifetime,
                ClockSkew = supabaseOptions.ClockSkew
            };
            options.MetadataAddress = $"{supabaseOptions.Url}/auth/v1/.well-known/openid-configuration";
            options.TokenValidationParameters.NameClaimType = "sub";
        });
        services.AddOptions<SupabaseJwtOptions>()
            .Bind(configuration.GetSection("Supabase"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}