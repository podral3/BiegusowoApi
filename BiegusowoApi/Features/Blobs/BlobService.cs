using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BiegusowoApi.Features.Blobs;

public class BlobService(
    ApplicationDbContext context,
    IFileStorageProvider fileStorageProvider,
    IOptions<FileStorageOptions> options) : IBlobService
{
    private readonly ApplicationDbContext _dbContext = context;
    private readonly FileStorageOptions _options = options.Value;
    private readonly IFileStorageProvider _fileStorageProvider = fileStorageProvider;

    public async Task<PresignedUploadResponse> CreatePresignedUploadsAsync(
        PresignedUploadRequest request,
        CancellationToken ct = default)
    {
        var uploads = await Task.WhenAll(
            request.Files.Select(async file =>
            {
                var id = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName);
                var key = $"{id}{extension}";

                var put = await _fileStorageProvider.CreatePresignedUploadUrlAsync(
                    key,
                    file.ContentType);

                var blob = new Blob
                {
                    Id = id,
                    StorageKey = key,
                    ContentType = file.ContentType,
                    Bucket = _options.Bucket,
                };

                return (Url: put.Url, Blob: blob);
            }));

        await _dbContext.Blobs.AddRangeAsync(
            uploads.Select(x => x.Blob),
            ct);

        await _dbContext.SaveChangesAsync(ct);

        return new PresignedUploadResponse(
            uploads
                .Select(x => new FileUploadResponse(x.Url, x.Blob.Id))
                .ToList());
    }

    public async Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default)
    {
        var blobIds = request.OrderIdPairs.Values.ToList();

        var blobs = await _dbContext.Blobs
            .Where(b => blobIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, ct);

        var results = new List<FileUploadResult>();

        foreach (var (order, blobId) in request.OrderIdPairs)
        {
            if (!blobs.TryGetValue(blobId, out var blob))
            {
                results.Add(new FileUploadResult(
                    null,
                    null,
                    "Blob not found"));

                continue;
            }

            var info = await _fileStorageProvider.GetObjectInfoAsync(
                blob.StorageKey,
                ct);

            if (info is null)
            {
                results.Add(new FileUploadResult(
                    blob.StorageKey,
                    null,
                    "File not found in storage"));

                continue;
            }

            blob.Uploaded = true;

            results.Add(new FileUploadResult(
                blob.StorageKey,
                order,
                null));
        }

        await _dbContext.SaveChangesAsync(ct);

        return new ConfirmUploadResult(results);
    }
}
