using Microsoft.EntityFrameworkCore;
using BiegusowoApi.Data;

namespace BiegusowoApi.Helpers.Composition;

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
                }));      

        return services;
    }
}
