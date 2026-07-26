namespace BiegusowoApi.Domain.Image;

public record ConfirmUploadRequest(
    Dictionary<string, string> OrderKeyPairs
);

public record ConfirmUploadResult(
    List<FileUploadResult> Result
);
public record FileUploadResult(
    string Key,
    Guid ImageId,
    string? Error
);