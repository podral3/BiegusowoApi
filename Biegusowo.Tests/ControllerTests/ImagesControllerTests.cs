using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Image;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Biegusowo.Tests.ControllerTests;

public class ImagesControllerTests(WebApplicationFactoryFixture factory)
    : BaseTests(factory)
{
    // ─── Upload ─────────────────────────────────────────────────────────────
    public async Task UploadPresigned_ShouldReturnPresignedUrls()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var listingId = "019f71d5-31a7-7fb7-8508-381e3881c3d9";
        var request = new PresignedUploadRequest(
            "listings",
            listingId,
            [
                new("test1.webp", "image/webp", 1920, 1080, 1024000, true),
                new("test2.webp", "image/webp", 1920, 1080, 2048000, false)
            ]
        );
        // Act
        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);
        // Assert
        response.Should().Be202Accepted();
        var result = await response.Content.ReadFromJsonAsync<PresignedUploadResponse>();
        await Verify(result);
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn401_WhenUnauthenticated()
    {
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [new("test1.webp", "image/webp", 1920, 1080, 1024000, true)]
        );

        var response = await PostAsJsonAsync(_client, "/api/images/presigned", request, CancellationToken);

        response.Should().Be401Unauthorized();
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn403_WhenListingBelongsToAnotherUser()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId); 
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a6-7752-ac76-5660301cf6ca",
            [new("test1.webp", "image/webp", 1920, 1080, 1024000, true)]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.Should().Be403Forbidden();
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenEntityIdDoesNotExist()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            Guid.NewGuid().ToString(),
            [new("test1.webp", "image/webp", 1920, 1080, 1024000, true)]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenImagesListIsEmpty()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest("listings", "019f71d5-31a7-7fb7-8508-381e3881c3d9", []);

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenContentTypeIsNotAllowed()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [new("malicious.exe", "application/x-msdownload", 0, 0, 1024, true)]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenFileSizeExceedsMax()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [new("huge.webp", "image/webp", 1920, 1080, 100_000_000, true)]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenMultipleImagesMarkedAsPrimary()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [
                new("test1.webp", "image/webp", 1920, 1080, 1024000, true),
                new("test2.webp", "image/webp", 1920, 1080, 1024000, true)
            ]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturn400_WhenFilenameContainsPathTraversal()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [new("../../etc/passwd.webp", "image/webp", 1920, 1080, 1024000, true)]
        );

        var response = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task UploadPresigned_ShouldReturnDistinctKeys_WhenCalledTwiceWithSameFilenames()
    {
        var client = _factory.CreateAuthenticatedClient(FirstUserId);
        var request = new PresignedUploadRequest(
            "listings",
            "019f71d5-31a7-7fb7-8508-381e3881c3d9",
            [new("test1.webp", "image/webp", 1920, 1080, 1024000, true)]
        );

        var response1 = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);
        var response2 = await PostAsJsonAsync(client, "/api/images/presigned", request, CancellationToken);

        var result1 = await response1.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);
        var result2 = await response2.Content.ReadFromJsonAsync<PresignedUploadResponse>(CancellationToken);

        result1!.Files.Select(i => i.Key)
            .Should().NotIntersectWith(result2!.Files.Select(i => i.Key));
    }

    // ─── Confirm ─────────────────────────────────────────────────────────────
}
