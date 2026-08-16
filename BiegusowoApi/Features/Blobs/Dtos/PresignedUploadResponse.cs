using Ardalis.Result;

namespace BiegusowoApi.Features.Blobs.Dtos;

public record PresignedUploadResponse(
    List<FileUploadResponse> Files,
    List<ValidationError> Errors
    );

public record FileUploadResponse(
    string PresignedUrl,
    Guid BlobId
    );

