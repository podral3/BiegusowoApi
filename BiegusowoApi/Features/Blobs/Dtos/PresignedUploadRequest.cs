namespace BiegusowoApi.Features.Blobs.Dtos;

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