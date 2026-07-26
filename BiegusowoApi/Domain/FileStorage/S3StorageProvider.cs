using Amazon.S3;
using Amazon.S3.Model;
using BiegusowoApi.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime;
using static BiegusowoApi.Domain.FileStorage.IFileStorageProvider;

namespace BiegusowoApi.Domain.FileStorage;

public class S3StorageProvider : IFileStorageProvider
{
    private readonly IAmazonS3 _s3;
    private readonly S3Options _options;

    public S3StorageProvider(IOptions<S3Options> options)
    {
        _options = options.Value;
        _s3 = new AmazonS3Client(
            _options.AccessKey,
            _options.SecretKey,
            new AmazonS3Config
            {
                ServiceURL = _options.PublicBaseUrl,
                AuthenticationRegion = _options.Region
            });
    }

    public async Task<PresignedPutUrl> CreatePresignedUploadUrlAsync(
        string objectKey, string contentType)
    {
        var expiresAt = DateTime.UtcNow.AddSeconds(_options.UrlExpiry);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = expiresAt,
            ContentType = contentType
        };

        var url = await _s3.GetPreSignedURLAsync(request);
        return new PresignedPutUrl(url, expiresAt);
    }

    public string GetPublicUrl(string objectKey)
    {
        if (!string.IsNullOrEmpty(_options.PublicBaseUrl))
            return $"{_options.PublicBaseUrl!.TrimEnd('/')}/{objectKey}";

        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{objectKey}";
    }

    public async Task<StorageObjectInfo?> GetObjectInfoAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var response = await _s3.GetObjectMetadataAsync(_options.BucketName, objectKey, ct);
            DateTimeOffset? lastModifiedOffset = response.LastModified.HasValue
            ? new DateTimeOffset(response.LastModified.Value, TimeSpan.Zero)
            : null;
            return new StorageObjectInfo(response.ContentLength, response.Headers.ContentType, lastModifiedOffset);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        await _s3.DeleteObjectAsync(_options.BucketName, objectKey, ct);
    }
}