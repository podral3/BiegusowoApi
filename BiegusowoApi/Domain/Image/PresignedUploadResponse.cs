namespace BiegusowoApi.Domain.Image;

public record PresignedUploadResponse(
    string Key,
    string PresignedUrl,
    Dictionary<string, string> Metadata
    );

