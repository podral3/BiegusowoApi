namespace BiegusowoApi.Domain.Image;

public record PresignedUploadRequest(
    Guid EntityId,
    List<PresignedUploadFile> Files
    );

public record PresignedUploadFile(
    string FileName,
    string ContentType,
    int ImageWidth,
    int ImageHeight,
    int FileSize,
    int Order
    );