using Scalar.AspNetCore;

namespace BiegusowoApi.Helpers.Composition;

public static class ConfigureScalar
{
    public static WebApplication UseScalar(this WebApplication app)
    {
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Biegusowo API");
            options.WithTheme(ScalarTheme.DeepSpace);
            options.AddPreferredSecuritySchemes("BearerAuth")
           .AddHttpAuthentication("BearerAuth", auth => { });
        }
        );
        app.MapGet("/", () => Results.Redirect("/scalar"));
        return app;
    }
}
