using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BiegusowoApi.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__mydb")
             ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=biegusowo";
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString, o => 
        o.UseNetTopologySuite());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
