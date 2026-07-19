using BiegusowoApi.Data;
using BiegusowoApi.Data.Seeding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Operation.Overlay.Validate;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;

namespace Biegusowo.Tests.Common;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public WebApplicationFactoryFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder("biegusowo-postgres")
            .WithDatabase("biegusowo")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString(), x =>
                {
                    x.UseNetTopologySuite();
                }));

            services.AddScoped<DataSeeder>();
        });
    }

    public HttpClient CreateAuthenticatedClient(int userId, params string[] roles)
    {
        return WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(new TestAuthContext
                {
                    UserId = userId,
                    Roles = roles.ToList()
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Test";
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }).CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        Console.WriteLine(_postgreSqlContainer.GetConnectionString());
        

        // Migrate
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        Console.WriteLine(db.Database.GetConnectionString());
        // Seed
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.Seed();

    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}
