namespace BiegusowoApi.Domain.Image;

public record PresignedUploadRequest(
    string FileName,
    string ContentType,
    int ImageWidth,
    int ImageHeight,
    int FileSize,
    string TableName,
    string OwnerEntityId,
    bool IsPrimary
    );
