namespace BiegusowoApi.Domain.Blobs.Service;

public interface IBlobService
{
    Task<PresignedUploadResponse> CreatePresignedUploadsAsync(
        List<PresignedFileInfo> request,
        CancellationToken ct = default);

    Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default);
}

public record PresignedFileInfo(
    string FileName,
    string ContentType,
    int FileSizeBytes,
    string Key
    );