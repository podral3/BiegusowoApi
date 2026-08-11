using BiegusowoApi.Features.Blobs.Dtos;

namespace BiegusowoApi.Features.Blobs;

public interface IBlobService
{
    Task<PresignedUploadResponse> CreatePresignedUploadsAsync(
        PresignedUploadRequest request,
        CancellationToken ct = default);

    Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default);
}