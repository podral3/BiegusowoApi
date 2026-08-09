using Testcontainers.PostgreSql;

namespace Biegusowo.Tests.Common.Containers;

public class PostgresTestContainer
{
    private readonly PostgreSqlContainer _container;

    public PostgresTestContainer()
    {
        _container = new PostgreSqlBuilder("biegusowo-postgres")
            .WithDatabase("biegusowo")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public Task StartAsync() => _container.StartAsync();
    public Task StopAsync() => _container.StopAsync();
    public string ConnectionString => _container.GetConnectionString();
}