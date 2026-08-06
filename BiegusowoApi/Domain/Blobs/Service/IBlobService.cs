using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Domain.Blobs.Service;

public interface IBlobService
{
    Task<PresignedUploadResponse> CreatePresignedUploadsAsync(
        Guid assetId,
        AssetType assetType,
        PresignedUploadRequest request,
        CancellationToken ct = default);

    Task<ConfirmUploadResult> ConfirmUploadsAsync(
        ConfirmUploadRequest request,
        CancellationToken ct = default);
}
