namespace BiegusowoApi.Domain.Blobs.Service;

public interface IBlobService
{
    Task<PresignedUploadResponse> CreatePresignedUploadsAsync(
        PresignedUploadRequest request,
        CancellationToken ct = default);

    Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default);
}