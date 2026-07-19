using Biegusowo.Tests.Common;
using AwesomeAssertions;
using System.Net;

namespace Biegusowo.Tests;

public class ListingsControllerTests : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly WebApplicationFactoryFixture _factory;

    public ListingsControllerTests(WebApplicationFactoryFixture factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteListing_WhenAuthenticated_ReturnsServerError_BecauseNotImplementedYet()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(userId: 1);

        // Act
        var response = await client.DeleteAsync("/api/listings/123");

        // Assert
        // NotImplementedException isn't caught anywhere, so ASP.NET Core's
        // default exception behavior surfaces it as a 500.
        // This test exists to flag the moment DeleteListing gets a real
        // implementation - when it starts failing, replace it with real assertions.
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task DeleteListing_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // no auth override -> real Keycloak scheme

        // Act
        var response = await client.DeleteAsync("/api/listings/123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}