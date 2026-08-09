using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Biegusowo.Tests.Common.Containers;

public class MailpitTestContainer
{
    public const string NetworkAlias = "mailpit";
    private readonly IContainer _container;

    public MailpitTestContainer(INetwork network)
    {
        _container = new ContainerBuilder("axllent/mailpit:v1.30")
            .WithNetwork(network)
            .WithNetworkAliases(NetworkAlias)
            .WithPortBinding(1025, true)
            .WithPortBinding(8025, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8025)))
            .Build();
    }

    public Task StartAsync() => _container.StartAsync();
    public Task StopAsync() => _container.StopAsync();

    // for host-side test assertions (HttpClient hitting the REST API)
    public string ApiBaseUrl => $"http://localhost:{_container.GetMappedPublicPort(8025)}";
}
