namespace BiegusowoApi.Domain.Image;

public record PresignedUploadResponse(
    List<FileUploadResponse> Files
    );

public record FileUploadResponse(
    string Key,
    string PresignedUrl,
    string ImageId,
    Dictionary<string, string> Metadata
    );

