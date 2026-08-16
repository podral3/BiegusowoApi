using Ardalis.Result;
using AwesomeAssertions;
using Biegusowo.Tests.Common;
using Biegusowo.Tests.Common.Fakes;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Blobs;
using BiegusowoApi.Features.Blobs.Dtos;
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

        PresignedUploadResponse presignedResponse = await blobService.CreatePresignedUploadsAsync(
            presignedRequest,
            CancellationToken);

        var blobId = presignedResponse.Files[0].BlobId;

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

    [Fact]
    public async Task CreatePresignedUploadsAsync_AllFilesInvalid_ReturnsInvalidResult()
    {
        // Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();

        var invalidFile = new PresignedUploadFile(
            "",                 // empty filename -> NotEmpty fails
            "image/png",        // wrong content type -> Equal fails
            0,                  // FileSizeBytes -> GreaterThan(0) fails
            0,                  // FileWidth -> GreaterThan(0) fails
            0);                 // FileHeight -> GreaterThan(0) fails

        var request = new PresignedUploadRequest([invalidFile]);

        // Act
        Result<PresignedUploadResponse> result =
            await objectService.CreatePresignedUploadsAsync(request, CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreatePresignedUploadsAsync_SomeFilesInvalid_ReturnsPartialSuccessWithErrors()
    {
        // Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();

        var validFile = new PresignedUploadFile(
            "validListingPhoto.webp",
            "image/webp",
            21_370_000,
            1920,
            1080);

        var invalidFile = new PresignedUploadFile(
            "invalidListingPhoto.webp",
            "image/webp",
            30 * 1024 * 1024,  // exceeds 25 MB max -> LessThanOrEqualTo fails
            1920,
            1080);

        var request = new PresignedUploadRequest([validFile, invalidFile]);

        // Act
        PresignedUploadResponse result =
            await objectService.CreatePresignedUploadsAsync(request, CancellationToken);

        // Assert

        result.Errors.Should().NotBeEmpty();
        result.Files.Should().HaveCount(1);
        var file = result.Files[0];

        Blob? blob = await GetQueryable<Blob>()
            .FirstOrDefaultAsync(b => b.Id == file.BlobId, CancellationToken);
        blob.Should().NotBeNull();
        blob.StorageKey.Should().NotContain(invalidFile.FileName);
    }

    [Theory]
    [InlineData("", "image/webp", 1_000_000, 1920, 1080)]      // empty filename
    [InlineData("photo.png", "image/png", 1_000_000, 1920, 1080)] // wrong content type
    [InlineData("photo.webp", "image/webp", 0, 1920, 1080)]     // zero size
    [InlineData("photo.webp", "image/webp", 1_000_000, 0, 1080)] // zero width
    [InlineData("photo.webp", "image/webp", 1_000_000, 1920, 0)] // zero height
    [InlineData("photo.webp", "image/webp", 1_000_000, 10001, 1080)] // width over max
    public async Task CreatePresignedUploadsAsync_SingleInvalidField_ReturnsInvalidResult(
        string fileName, string contentType, int fileSize, int width, int height)
    {
        // Arrange
        var scope = _factory.Services.CreateScope();
        var objectService = scope.ServiceProvider.GetRequiredService<IBlobService>();

        var invalidFile = new PresignedUploadFile(fileName, contentType, fileSize, width, height);
        var request = new PresignedUploadRequest([invalidFile]);

        // Act
        Result<PresignedUploadResponse> result =
       await objectService.CreatePresignedUploadsAsync(request, CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().NotBeEmpty();
    }
}