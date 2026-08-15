using AwesomeAssertions;
using Biegusowo.Tests.Common;
using Biegusowo.Tests.Common.Fakes;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Accounts.Dtos;
using BiegusowoApi.Features.Auth;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Features.Users.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Ocsp;
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
        var client = _factory.CreateClient();
        var secondUserId = "00000000-0000-0000-0000-000000000002";
        // Act
        var response = await client.GetAsync($"/api/profiles/{secondUserId}",
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
        var response = await client.GetAsync($"/api/profiles/{nonExistentUserId}",
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
        var response = await client.GetAsync("/api/profiles/me",
            CancellationToken);
        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<ProfilePageResponse>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task GetMyProfile_ProfileNotSetUp_ReturnsOnboardingRequired()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000000999");
        //Act
        var response = await client.GetAsync("/api/profiles/me", CancellationToken);
        //Assert
        response.Should().Be200Ok()
        .And.BeAs(new
        {
            authenticated = true,
            onboardingRequired = true
        });
    }

    [Fact]
    public async Task UpdateUserInfo_ReturnsOk()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000000040");
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
        var response = await client.PatchAsync("/api/profiles/me", content, CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var result = await response.Content.ReadFromJsonAsync<ProfilePageResponse>(CancellationToken);
        result.Should().NotBeNull();
        result.User.Bio.Should().Be("Updated Bio");
        result.User.DisplayName.Should().Be("Updated Display Name");
        result.User.PhoneNumber.Should().Be("123456789");
    }

    [Fact]
    public async Task UpdateUserInfo_InvalidPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000000051");
        var ops = new[]
        {
            new { op = "replace", path = "/PhoneNumber", value = "2137" }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(ops);

        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");
        // Act
        var response = await client.PatchAsync("/api/profiles/me", content, CancellationToken);
        // Assert
        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task SetupAccountSetsUpAccount()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000001111");
        var request = new SetupAccountRequest("Setup User", "This is a new user.", "987654321", "Warszawa", 2);
        // Act
        var response = await client.PostAsJsonAsync("/api/profiles/setup", request, CancellationToken);
        // Assert
        response.Should().Be201Created();
        var result = await response.Content.ReadFromJsonAsync<UserDto>(CancellationToken);
        await Verify(result);
    }

    [Fact]
    public async Task SetupAccount_AlreadySetup_ReturnsConflict()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new SetupAccountRequest("Setup User", "This is a new user.", "987654321", "Warszawa", 2);
        // Act
        var response = await client.PostAsJsonAsync("/api/profiles/setup", request, CancellationToken);
        // Assert
        response.Should().Be409Conflict();
    }

    /// <summary>
    /// Requests a presigned upload via the given endpoint, simulates the client uploading
    /// the resulting object to the fake storage provider, and returns the created blob id.
    /// </summary>
    private async Task<Guid> CreateAndUploadPresignedImageAsync(
        HttpClient client,
        string presignedEndpoint,
        string fileName,
        string contentType)
    {
        var presignedFile = new PresignedUploadFile(fileName, contentType, 512_000, 512, 512);

        var presignedResponse = await client.PostAsJsonAsync(presignedEndpoint, presignedFile, CancellationToken);
        presignedResponse.Should().Be200Ok();

        var presigned = await presignedResponse.Content
            .ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        presigned.Should().NotBeNull();
        var blobId = presigned!.Files.Single().BlobId;

        var blob = await GetQueryable<Blob>()
            .AsNoTracking()
            .SingleAsync(b => b.Id == blobId, CancellationToken);

        using var scope = _factory.Services.CreateScope();
        var fakeStorage = scope.ServiceProvider.GetRequiredService<FakeStorageProvider>();
        fakeStorage.SimulateUpload(blob.StorageKey, presignedFile.FileSizeBytes, presignedFile.ContentType);

        return blobId;
    }

    // ---------------------------------------------------------------------
    // Avatar
    // ---------------------------------------------------------------------

    [Fact]
    public async Task UploadAvatarPresigned_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadFile("avatar.webp", "image/webp", 512_000, 512, 512);

        // Act
        var response = await client.PostAsJsonAsync("/api/profiles/me/avatar/presigned", request, CancellationToken);

        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        result.Should().NotBeNull();
        result!.Files.Should().HaveCount(1);
    }

    [Fact]
    public async Task ConfirmAvatarUpload_ReturnsOkAndUpdatesUser()
    {
        // Arrange
        string userId = UserId(5);
        var client = _factory.CreateAuthenticatedClient(userId);
        var presignedRequest = new PresignedUploadFile("avatarConfirm.webp", "image/webp", 512_000, 512, 512);
        var presignedResponse = await client.PostAsJsonAsync("/api/profiles/me/avatar/presigned", presignedRequest, CancellationToken);
        var presignedResult = await presignedResponse.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        var blobId = presignedResult!.Files.Single().BlobId;

        // Act
        var response = await client.PutAsJsonAsync("/api/profiles/me/avatar", blobId, CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var user = await GetQueryable<User>()
            .AsNoTracking()
            .SingleAsync(u => u.Id == Guid.Parse(userId), CancellationToken);
        user.AvatarFileName.Should().Contain(blobId.ToString());
    }

    [Fact]
    public async Task ConfirmAvatarUpload_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000002137");

        // Act
        var response = await client.PutAsJsonAsync("/api/profiles/me/avatar", Guid.NewGuid(), CancellationToken);

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task DeleteAvatar_ReturnsOkAndClearsAvatar()
    {
        // Arrange
        string userId = UserId(6);
        var client = _factory.CreateAuthenticatedClient(userId);
        var presignedRequest = new PresignedUploadFile("avatarDelete.webp", "image/webp", 512_000, 512, 512);
        var presignedResponse = await client.PostAsJsonAsync("/api/profiles/me/avatar/presigned", presignedRequest, CancellationToken);
        var presignedResult = await presignedResponse.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        var confirmRequest = await client.PutAsJsonAsync("/api/profiles/me/avatar", presignedResult!.Files.Single().BlobId, CancellationToken);

        // Act
        var response = await client.DeleteAsync("/api/profiles/me/avatar", CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var user = await GetQueryable<User>()
            .AsNoTracking()
            .SingleAsync(u => u.Id == Guid.Parse(userId), CancellationToken);
        user.AvatarFileName.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAvatar_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000002137");

        // Act
        var response = await client.DeleteAsync("/api/profiles/me/avatar", CancellationToken);

        // Assert
        response.Should().Be404NotFound();
    }

    // ---------------------------------------------------------------------
    // Background image
    // ---------------------------------------------------------------------

    [Fact]
    public async Task UploadBackgroundImagePresigned_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadFile("background.webp", "image/webp", 1_024_000, 1920, 1080);

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/profiles/me/background-image/presigned", request, CancellationToken);

        // Assert
        response.Should().Be200Ok();
        var result = await response.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        result.Should().NotBeNull();
        result!.Files.Should().HaveCount(1);
    }

    [Fact]
    public async Task ConfirmBackgroundImageUpload_ReturnsOkAndUpdatesUser()
    {
        // Arrange
        string userId = UserId(7);
        var client = _factory.CreateAuthenticatedClient(userId);
        var presignedRequest = new PresignedUploadFile("backgroundConfirm.webp", "image/webp", 512_000, 512, 512);
        var presignedResponse = await client.PostAsJsonAsync("/api/profiles/me/background-image/presigned", presignedRequest, CancellationToken);
        var presignedResult = await presignedResponse.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        var blobId = presignedResult!.Files.Single().BlobId;

        // Act
        var response = await client.PutAsJsonAsync("/api/profiles/me/background-image", presignedResult!.Files.Single().BlobId, CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var user = await GetQueryable<User>()
            .AsNoTracking()
            .SingleAsync(u => u.Id == Guid.Parse(userId), CancellationToken);
        user.BackgroundFileName.Should().Contain(blobId.ToString());
    }

    [Fact]
    public async Task ConfirmBackgroundImageUpload_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000002137");

        // Act
        var response = await client.PutAsJsonAsync(
            "/api/profiles/me/background-image", Guid.NewGuid(), CancellationToken);

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task DeleteBackgroundImage_ReturnsOkAndClearsBackgroundImage()
    {
        // Arrange
        string userId = UserId(8);
        var client = _factory.CreateAuthenticatedClient(userId);
        var presignedRequest = new PresignedUploadFile("backgroundDelete.webp", "image/webp", 512_000, 512, 512);
        var presignedResponse = await client.PostAsJsonAsync("/api/profiles/me/background-image/presigned", presignedRequest, CancellationToken);
        var presignedResult = await presignedResponse.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        var confirmRequest = await client.PutAsJsonAsync("/api/profiles/me/background-image", presignedResult!.Files.Single().BlobId, CancellationToken);

        // Act
        var response = await client.DeleteAsync("/api/profiles/me/background-image", CancellationToken);

        // Assert
        response.Should().Be200Ok();

        var user = await GetQueryable<User>()
            .AsNoTracking()
            .SingleAsync(u => u.Id == Guid.Parse(userId), CancellationToken);
        user.BackgroundFileName.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteBackgroundImage_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("00000000-0000-0000-0000-000000002137");

        // Act
        var response = await client.DeleteAsync("/api/profiles/me/background-image", CancellationToken);

        // Assert
        response.Should().Be404NotFound();
    }

}
