using AwesomeAssertions;
using Biegusowo.Tests.Common;
using Biegusowo.Tests.Common.Fakes;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Blobs.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Biegusowo.Tests.ServiceTests;

public class BlobServiceTests(WebApplicationFactoryFixture factory)
    : BaseTests(factory)
{
    [Fact]
    public async Task CreatePresignedUploadsAsync_ShouldReturnPresignedUploadResponseAndCreateDbRecord()
    {
        //Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var singleFile = new PresignedUploadFile("listingPhoto.webp", "image/webp", 21370000, 1920, 1080);
        PresignedUploadRequest request = new([singleFile]);

        //Act
        PresignedUploadResponse result = await objectService.CreatePresignedUploadsAsync(request, CancellationToken);

        //Assert   
        result.Files.Count.Should().Be(1);
        Blob? blob = await GetQueryable<Blob>()
            .FirstOrDefaultAsync(b => b.Id == result.Files[0].BlobId, CancellationToken);
        blob.Should().NotBeNull();
    }
    [Fact]
    public async Task ConfirmUploadsAsync_ShouldReturnConfirmUploadResponseAndUpdateDbRecord()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var fakeStorage = scope.ServiceProvider.GetRequiredService<FakeStorageProvider>();

        var file = new PresignedUploadFile(
            "listingPhoto1.webp",
            "image/webp",
            21_370_000,
            1920,
            1080);

        var presignedRequest = new PresignedUploadRequest([file]);

        var presignedResponse = await blobService.CreatePresignedUploadsAsync(
            presignedRequest,
            CancellationToken);

        var blobId = presignedResponse.Files.Single().BlobId;

        var blob = await GetQueryable<Blob>()
            .AsNoTracking()
            .SingleAsync(b => b.Id == blobId, CancellationToken);

        // Simulate the client actually uploading the object
        fakeStorage.SimulateUpload(
            blob.StorageKey,
            file.FileSizeBytes,
            file.ContentType);

        var confirmRequest = new ConfirmUploadRequest(
            new Dictionary<int, Guid>
            {
                [0] = blobId
            });

        // Act
        var result = await blobService.ConfirmUploadsAsync(
            confirmRequest,
            CancellationToken);

        // Assert
        result.Result.Should().HaveCount(1);

        result.Result[0].Key.Should().Be(blob.StorageKey);
        result.Result[0].Order.Should().Be(0);
        result.Result[0].Error.Should().BeNull();

        var savedBlob = await GetQueryable<Blob>()
            .AsNoTracking()
            .SingleAsync(b => b.Id == blobId, CancellationToken);

        savedBlob.Uploaded.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmUploadAsync_NonExistendId_ReturnsErrors()
    {
        //Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();

        ConfirmUploadRequest confirmRequest = new(
            OrderIdPairs: new Dictionary<int, Guid>
            {
                [0] = Guid.NewGuid()
            });
        //Act
        ConfirmUploadResult confirmResults = await objectService.ConfirmUploadsAsync(confirmRequest, CancellationToken);

        confirmResults.Result.Count.Should().Be(1);
        confirmResults.Result[0].Key.Should().BeNull();
        confirmResults.Result[0].Error.Should().NotBeNullOrEmpty();
    }
}
