namespace BiegusowoApi.Domain.Image.Service;

public class StorageObjectService : IBlobService
{
    public Task<PresignedUploadResponse> CreatePresignedUploadsAsync(PresignedUploadRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ConfirmUploadResult> ConfirmUploadsAsync(ConfirmUploadRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

}
