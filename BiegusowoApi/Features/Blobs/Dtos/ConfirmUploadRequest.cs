namespace BiegusowoApi.Features.Blobs.Dtos;

public record ConfirmUploadRequest(
    Dictionary<int, Guid> OrderIdPairs
);

public record ConfirmUploadResult(
    List<FileUploadResult> Result
);
public record FileUploadResult(
    string? Key,
    int? Order,
    string? Error
);