using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Shared.Options;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using static BiegusowoApi.Features.Blobs.IFileStorageProvider;

namespace BiegusowoApi.Features.Blobs;

public class BlobService(
    ApplicationDbContext context,
    IFileStorageProvider fileStorageProvider,
    IOptions<FileStorageOptions> options,
    ILogger<BlobService> logger) : IBlobService
{
    private readonly ApplicationDbContext _dbContext = context;
    private readonly FileStorageOptions _options = options.Value;
    private readonly IFileStorageProvider _fileStorageProvider = fileStorageProvider;
    private readonly ILogger<BlobService> _logger = logger;

    public async Task<Result<PresignedUploadResponse>> CreatePresignedUploadsAsync(
        PresignedUploadRequest request,
        CancellationToken ct = default)
    {
        PresignedUploadFileValidator validator = new();
        var results = await Task.WhenAll(
            request.Files.Select(file => validator.ValidateAsync(file, ct)));

        var validFiles = request.Files
            .Zip(results, (file, result) => (file, result))
            .Where(x => x.result.IsValid)
            .Select(x => x.file)
            .ToList();

        var validationErrors = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.AsErrors())
            .ToList();

        if (validFiles.Count == 0)
        {
            return Result<PresignedUploadResponse>.Invalid(validationErrors);
        }

        var uploads = await Task.WhenAll(
            validFiles.Select(async file =>
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
            [.. uploads.Select(x => new FileUploadResponse(x.Url, x.Blob.Id))],
            validationErrors);
    }

    public async Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default)
    {
        var blobIds = request.OrderIdPairs.Values.Distinct().ToList();

        var blobs = await _dbContext.Blobs
            .Where(b => blobIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, ct);

        var infoByBlobId = new Dictionary<Guid, StorageObjectInfo?>();
        await Task.WhenAll(blobs.Values.Select(async blob =>
        {
            var info = await _fileStorageProvider.GetObjectInfoAsync(blob.StorageKey, ct);
            lock (infoByBlobId)
            {
                infoByBlobId[blob.Id] = info;
            }
        }));

        var results = new List<FileUploadResult>();

        foreach (var (order, blobId) in request.OrderIdPairs)
        {
            if (!blobs.TryGetValue(blobId, out var blob))
            {
                results.Add(new FileUploadResult(null, null, "Blob not found"));
                continue;
            }

            var info = infoByBlobId[blob.Id];
            if (info is null)
            {
                _logger.LogWarning("Blob with ID {BlobId} and storage key {StorageKey} not found in storage", blob.Id, blob.StorageKey);
                results.Add(new FileUploadResult(blob.StorageKey, null, "File not found in storage"));
                continue;
            }

            blob.Uploaded = true;
            results.Add(new FileUploadResult(blob.StorageKey, order, null));
        }

        await _dbContext.SaveChangesAsync(ct);

        return new ConfirmUploadResult(results);
    }
}
