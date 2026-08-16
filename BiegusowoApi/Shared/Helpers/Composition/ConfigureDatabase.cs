using Microsoft.EntityFrameworkCore;
using BiegusowoApi.Data;

namespace BiegusowoApi.Shared.Helpers.Composition;

public static class ConfigureDatabase
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("biegusowo")!;
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString, x =>
                {
                    
                    x.UseNetTopologySuite();
                    x.EnableRetryOnFailure(3);
                    x.CommandTimeout(30);
                }));      

        return services;
    }
}
