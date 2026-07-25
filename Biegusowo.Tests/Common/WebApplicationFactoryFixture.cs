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
    private IServiceScopeFactory _scopeFactory;

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
    public IServiceScope CreateScope() => _scopeFactory.CreateScope();
    public HttpClient CreateAuthenticatedClient(string userId, params string[] roles)
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

    public async ValueTask InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();        

        // Migrate
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        // Seed
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.Seed();

        _scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        GC.SuppressFinalize(this);
    }
}
