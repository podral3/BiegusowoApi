using Biegusowo.Tests.Common;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Biegusowo.Tests.ServiceTests;

public class SeedingTests(WebApplicationFactoryFixture factory) 
    : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly WebApplicationFactoryFixture _factory = factory;

    [Fact]
    public async Task SeedingShouldSeed()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

        Assert.True(await dbContext.Users.AnyAsync());
    }
}
