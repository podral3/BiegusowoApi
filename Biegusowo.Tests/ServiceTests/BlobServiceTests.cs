using AwesomeAssertions;
using Biegusowo.Tests.Common;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Image;
using BiegusowoApi.Domain.Image.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biegusowo.Tests.ServiceTests;

public class BlobServiceTests(WebApplicationFactoryFixture factory)
    : BaseTests(factory)
{
    [Fact]
    public async Task CreatePresignedUploadsAsync_ShouldReturnPresignedUploadResponseAndCreateDbRecord()
    {
        //Arrange
        var objectService = _factory.Services.GetRequiredService<IBlobService>();
        var singleFile = new PresignedUploadFile("listingPhoto.webp", "image/webp", 21370000);
        var request = new PresignedUploadRequest([singleFile]);

        //Act
        PresignedUploadResponse result = await objectService.CreatePresignedUploadsAsync(request, CancellationToken);

        //Assert   
        result.Files.Count.Should().Be(1);
        Blob? blob = await GetQueryable<Blob>()
            .FirstOrDefaultAsync(b => b.Id == result.Files[0].ImageId, CancellationToken);
        blob.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfirmUploadsAsync_ShouldReturnConfirmUploadResponseAndUpdateDbRecord()
    {
        //Arrange
        var objectService = _factory.Services.GetRequiredService<IBlobService>();
        var singleFile = new PresignedUploadFile("listingPhoto.webp", "image/webp", 21370000);

        var presignedRequest = new PresignedUploadRequest([singleFile]);
        PresignedUploadResponse presignedResponse = await objectService.CreatePresignedUploadsAsync(presignedRequest, CancellationToken);
        var kvp = presignedResponse.Files
            .Select((f, index) => new { Key = (index + 1).ToString(), Value = f.Key })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        ConfirmUploadRequest confirmRequest = new(kvp);
        //Act
        ConfirmUploadResult confirmResults = await objectService.ConfirmUploadsAsync(confirmRequest, CancellationToken);
        //Assert   
        confirmResults.Result.Count.Should().Be(1);
        Blob? blob = await GetQueryable<Blob>()
            .FirstOrDefaultAsync(b => b.Id == confirmResults.Result[0].ImageId, CancellationToken);
        blob.Should().NotBeNull();
        blob!.Uploaded.Should().BeTrue();
    }
}
