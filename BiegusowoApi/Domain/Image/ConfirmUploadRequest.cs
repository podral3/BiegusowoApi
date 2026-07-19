namespace BiegusowoApi.Domain.Image;

public record ConfirmUploadRequest(
    string TableName,
    string EntityId,
    List<string> Keys
);