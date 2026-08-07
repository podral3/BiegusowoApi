using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static BiegusowoApi.Domain.FileStorage.IFileStorageProvider;

namespace BiegusowoApi.Domain.Blobs.Service;

public class BlobService(
    ApplicationDbContext context,
    IFileStorageProvider fileStorageProvider,
    IOptions<FileStorageOptions> options) : IBlobService
{
    private readonly ApplicationDbContext _dbContext = context;
    private readonly FileStorageOptions _options = options.Value;
    private readonly IFileStorageProvider _fileStorageProvider = fileStorageProvider;

    public async Task<PresignedUploadResponse> CreatePresignedUploadsAsync(List<PresignedFileInfo> request, CancellationToken ct = default)
    {
        List<Blob> blobs = [.. request.Select(file => new Blob
        {
            StorageKey = file.Key,
            ContentType = file.ContentType,
            Bucket = _options.Bucket,
        })];

        await _dbContext.Blobs.AddRangeAsync(blobs, ct);
        await _dbContext.SaveChangesAsync(ct);

        List<FileUploadResponse> presignedUploadResponse = [];
        for (int i = 0; i < request.Count; i++)
        {
            PresignedPutUrl put = await _fileStorageProvider.CreatePresignedUploadUrlAsync(request[i].Key, blobs[i].ContentType);
            presignedUploadResponse.Add(new FileUploadResponse(put.Url, blobs[i].Id));
        }

        return new PresignedUploadResponse(presignedUploadResponse);
    }

    public async Task<ConfirmUploadResult> ConfirmUploadsAsync(ConfirmUploadRequest request, CancellationToken ct = default)
    {
        List<FileUploadResult> results = [];
        foreach (var kvp in request.OrderIdPairs)
        {
            Blob? blob = await _dbContext.Blobs.FirstOrDefaultAsync(b => b.Id == kvp.Value, ct);
            if(blob == null)
            {
                results.Add(new FileUploadResult(null, null, Error: "Blob not found"));
                continue;   
            }
            StorageObjectInfo? info =await _fileStorageProvider.GetObjectInfoAsync(blob.StorageKey, ct);
            if (info == null)
            {
                results.Add(new FileUploadResult(blob.StorageKey, null, Error: "File not found in storage"));
                continue;
            }
            blob.Uploaded = true;
            results.Add(new FileUploadResult(blob.StorageKey, kvp.Key, null));
        }
        await _dbContext.SaveChangesAsync(ct);
        return new ConfirmUploadResult(results);
    }

    
}
