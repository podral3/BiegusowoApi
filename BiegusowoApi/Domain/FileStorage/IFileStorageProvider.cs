namespace BiegusowoApi.Domain.FileStorage;

public interface IFileStorageProvider
{
    public record PresignedPutUrl(string Url, DateTimeOffset ExpiresAt);

    public record StorageObjectInfo(long SizeBytes, string? ContentType, DateTimeOffset? LastModified);
    
    Task<PresignedPutUrl> CreatePresignedUploadUrlAsync(
      string objectKey, string contentType);

    string GetPublicUrl(string objectKey);

    Task<StorageObjectInfo?> GetObjectInfoAsync(string objectKey, CancellationToken ct = default);

    Task DeleteAsync(string objectKey, CancellationToken ct = default);
}
