namespace BiegusowoApi.Domain.Blobs;

public record PresignedUploadRequest(
    List<PresignedUploadFile> Files
    );

public record PresignedUploadFile(
    string FileName,
    string ContentType,
    int FileSizeBytes,
    int FileWidth,
    int FileHeight
    );