using Biegusowo.Tests.Common.Containers;
using Biegusowo.Tests.Common.Fakes;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Seeding;
using BiegusowoApi.Features.Blobs;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Biegusowo.Tests.Common;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
  
    private readonly PostgresTestContainer _postgres;
    private IServiceScopeFactory _scopeFactory;

    public WebApplicationFactoryFixture()
    {
        _postgres = new PostgresTestContainer();
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
                options.UseNpgsql(_postgres.ConnectionString, x =>
                {
                    x.UseNetTopologySuite();
                }));

            var fileStorageDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IFileStorageProvider));
            if (fileStorageDescriptor != null)
                services.Remove(fileStorageDescriptor);
            services.AddScoped<FakeStorageProvider>();

            services.AddScoped<IFileStorageProvider>(sp =>
                sp.GetRequiredService<FakeStorageProvider>());

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
        await _postgres.StartAsync();
            

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
        await _postgres.StopAsync();
    }
}
