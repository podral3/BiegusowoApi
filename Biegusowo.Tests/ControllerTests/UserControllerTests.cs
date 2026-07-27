using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Domain.Dtos.ProfilePage;
using Microsoft.AspNetCore.JsonPatch;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Biegusowo.Tests.ControllerTests;

public class UserControllerTests(WebApplicationFactoryFixture factory)
    : BaseTests(factory)
{
    [Fact]
    public async Task GetProfile_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var secondUserId = "00000000-0000-0000-0000-000000000002";
        // Act
        var response = await client.GetAsync($"/api/users/{secondUserId}",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ProfilePageResponse>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task GetProfile_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var nonExistentUserId = "00000000-0000-0000-0000-000000002137";
        // Act
        var response = await client.GetAsync($"/api/users/{nonExistentUserId}",
            CancellationToken);
        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task GetMyProfile_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        // Act
        var response = await client.GetAsync("/api/users/me",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ProfilePageResponse>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task UpdateUserInfo_ReturnsOk()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var ops = new[]
        {
            new { op = "replace", path = "/DisplayName", value = "Updated Display Name" },
            new { op = "replace", path = "/Bio", value = "Updated Bio" },
            new { op = "replace", path = "/PhoneNumber", value = "123456789" }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(ops);

        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");
        // Act
        var response = await client.PatchAsync("/api/users/me", content, CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var result = await response.Content.ReadFromJsonAsync<ProfilePageResponse>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task UpdateUserInfo_InvalidPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-0000000051");
        var patchDoc = new JsonPatchDocument<UserPatchRequest>();
        patchDoc.Replace(u => u.PhoneNumber, "InvalidPhoneNumber");
        // Act
        var response = await client.PatchAsJsonAsync("/api/users/me", patchDoc, CancellationToken);
        // Assert
        response.Should().Be400BadRequest();
    }
}
