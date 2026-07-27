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
        var singleFile = new PresignedFileInfo("listingPhoto.webp", "image/webp", 21370000, "/key/that/will/work");
        List<PresignedFileInfo> request = [singleFile];

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
        //Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var singleFile = new PresignedFileInfo("listingPhoto1.webp", "image/webp", 21370000, "/key/that/will/work1");

        var fakeStorage = scope.ServiceProvider.GetRequiredService<FakeStorageProvider>();
        fakeStorage.SimulateUpload(singleFile.Key, singleFile.FileSizeBytes, singleFile.ContentType);

        List<PresignedFileInfo> presignedRequest = [singleFile];
        PresignedUploadResponse presignedResponse = await objectService.CreatePresignedUploadsAsync(presignedRequest, CancellationToken);
        var kvp = presignedResponse.Files
            .Select((f, index) => new { Key = index, Value = f.BlobId })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        ConfirmUploadRequest confirmRequest = new(kvp);
        //Act
        ConfirmUploadResult confirmResults = await objectService.ConfirmUploadsAsync(confirmRequest, CancellationToken);
        //Assert   
        confirmResults.Result.Count.Should().Be(1);
        confirmResults.Result[0].Key.Should().Be(singleFile.Key);
        confirmResults.Result[0].Order.Should().Be(0);
        Blob? blob = await GetQueryable<Blob>()
            .FirstOrDefaultAsync(b => b.StorageKey == confirmResults.Result[0].Key, CancellationToken);
        blob.Should().NotBeNull();
        blob!.Uploaded.Should().BeTrue();
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
