using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.Keycloak;

namespace Biegusowo.Tests.Common.Containers;

public class KeycloakTestContainer
{
    private readonly IContainer _container;

    public KeycloakTestContainer(INetwork network)
    {
        var testAssemblyLocation = typeof(KeycloakTestContainer).Assembly.Location;
        var testProjectDir = Path.GetDirectoryName(testAssemblyLocation)!;

        var realmJsonPath = Path.GetFullPath(
            Path.Combine(testProjectDir, "../../../../Keycloak/realm-export.json"));

        if (!File.Exists(realmJsonPath))
            throw new FileNotFoundException("Realm JSON not found", realmJsonPath);

        _container = new KeycloakBuilder("keycloak/keycloak:26.7")
            .WithNetwork(network)
            .WithBindMount(realmJsonPath, "/opt/keycloak/data/import/realm-export.json")
            //.WithCommand("start-dev", "--import-realm") // must be passed as separate args
            .Build();
    }

    public Task StartAsync() => _container.StartAsync();
    public Task StopAsync() => _container.StopAsync();

    // for the app under test (running in-process) to reach Keycloak
    public string AuthorityUrl => $"http://localhost:{_container.GetMappedPublicPort(8080)}/realms/biegusowo";
}