using BiegusowoApi.Domain.FileStorage;
using BiegusowoApi.Domain.Image;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static BiegusowoApi.Domain.FileStorage.IFileStorageProvider;

namespace Biegusowo.Tests.Common.Fakes;

/// <summary>
/// In-memory fake of IStorageProvider for unit testing ImageService without
/// hitting real S3/R2. "Uploading" a file is simulated via SimulateUpload -
/// call it after presign to make GetObjectInfoAsync succeed on confirm,
/// the same way a real client PUTting the file to the presigned URL would.
/// </summary>
public class FakeStorageProvider : IFileStorageProvider
{
    private readonly ConcurrentDictionary<string, StorageObjectInfo> _objects = new();

    /// <summary>Every key CreatePresignedUploadUrlAsync was called with, in order.</summary>
    public List<string> PresignedKeys { get; } = new();

    /// <summary>Every key DeleteAsync was called with, in order.</summary>
    public List<string> DeletedKeys { get; } = new();

    public Task<PresignedPutUrl> CreatePresignedUploadUrlAsync(string objectKey, string contentType)
    {
        PresignedKeys.Add(objectKey);

        return Task.FromResult(new PresignedPutUrl(
            Url: $"https://fake-upload.test/{objectKey}",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15)
        ));
    }

    public string GetPublicUrl(string objectKey) => $"https://fake-public.test/{objectKey}";

    public Task<StorageObjectInfo?> GetObjectInfoAsync(string objectKey, CancellationToken ct = default)
    {
        _objects.TryGetValue(objectKey, out var info);
        return Task.FromResult(info);
    }

    public Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        DeletedKeys.Add(objectKey);
        _objects.TryRemove(objectKey, out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Test helper: pretend a client successfully PUT this file to the presigned
    /// URL, so a subsequent ConfirmUploadsAsync call finds it via GetObjectInfoAsync.
    /// </summary>
    public void SimulateUpload(string objectKey, long sizeBytes, string contentType = "image/jpeg")
    {
        _objects[objectKey] = new StorageObjectInfo(sizeBytes, contentType, DateTimeOffset.UtcNow);
    }
}
