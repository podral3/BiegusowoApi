namespace BiegusowoApi.Domain.Blobs;

public record PresignedUploadResponse(
    List<FileUploadResponse> Files
    );

public record FileUploadResponse(
    string PresignedUrl,
    Guid BlobId
    );

