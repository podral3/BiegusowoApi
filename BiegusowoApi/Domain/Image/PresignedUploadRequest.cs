namespace BiegusowoApi.Domain.Image;

public record PresignedUploadRequest(
    List<PresignedUploadFile> Files
    );

public record PresignedUploadFile(
    string FileName,
    string ContentType,
    int FileSize
    );