using Microsoft.OpenApi;

namespace BiegusowoApi.Helpers.Composition;

public static class ConfigureOpenApi
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // 1. Schema Transformer
            options.AddSchemaTransformer((schema, context, ct) =>
            {
                if (schema.Type.HasValue &&
                (schema.Type.Value.HasFlag(JsonSchemaType.Integer) || schema.Type.Value.HasFlag(JsonSchemaType.Number)) &&
                schema.Type.Value.HasFlag(JsonSchemaType.String))
                {
                    schema.Type &= ~JsonSchemaType.String;
                    schema.Pattern = null;
                }
                return Task.CompletedTask;
            });

            // 2. Document Transformer
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "Biegusowo API";
                document.Info.Version = "v1";
                document.Info.Description = "API for Biegusowo App";

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes.Add("BearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token"
                });

                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("BearerAuth", document)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }
}