namespace BiegusowoApi.Features.Blobs.Dtos;

public record PresignedUploadResponse(
    List<FileUploadResponse> Files
    );

public record FileUploadResponse(
    string PresignedUrl,
    Guid BlobId
    );

