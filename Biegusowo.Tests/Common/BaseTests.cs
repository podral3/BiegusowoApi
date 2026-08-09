using BiegusowoApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Biegusowo.Tests.Common;

public class BaseTests : IClassFixture<WebApplicationFactoryFixture>
{
    protected readonly WebApplicationFactoryFixture _factory;
    protected HttpClient _client;
    protected readonly IServiceScope _serviceScope;
    private readonly ApplicationDbContext _dbContext;

    protected readonly string FirstUserId = "00000000-0000-0000-0000-000000000001";

    public BaseTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _serviceScope = _factory.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected IQueryable<T> GetQueryable<T>() where T : class => _dbContext.Set<T>().AsNoTracking();
    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data, CancellationToken cancellationToken)
        => await _client.PostAsJsonAsync(url, data, cancellationToken);

    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(HttpClient client, string url,  T data, CancellationToken cancellationToken)
        => await client.PostAsJsonAsync(url, data, cancellationToken);

    protected async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        await _dbContext.AddAsync(entity, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

    protected async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        await _dbContext.AddRangeAsync(entities, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

    protected async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

}
