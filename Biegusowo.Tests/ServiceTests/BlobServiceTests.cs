using AwesomeAssertions;
using Biegusowo.Tests.Common;
using Biegusowo.Tests.Common.Fakes;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
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
        Guid guid = Guid.Parse("00000000-0000-0000-0000-000000000001");

        //Act
        PresignedUploadResponse result = await objectService.CreatePresignedUploadsAsync(
            guid, BiegusowoApi.Data.Types.AssetType.Listing, request, CancellationToken);

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
            "listingPhoto.webp",
            "image/webp",
            21_370_000,
            1920,
            1080);

        var assetId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        var request = new PresignedUploadRequest([file]);

        var presignedResponse = await blobService.CreatePresignedUploadsAsync(
            assetId,
            AssetType.Listing,
            request,
            CancellationToken);

        var blobId = presignedResponse.Files.Single().BlobId;

        var blob = await GetQueryable<Blob>()
            .SingleAsync(b => b.Id == blobId, CancellationToken);

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

        var confirmedBlob = await GetQueryable<Blob>()
            .AsNoTracking()
            .SingleAsync(b => b.Id == blobId, CancellationToken);

        confirmedBlob.Uploaded.Should().BeTrue();
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
