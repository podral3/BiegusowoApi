using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Data.Models;

public class Blob
{
    public Guid Id { get; set; }
    public string StorageKey { get; set; }
    public string ContentType { get; set; }
    public string Bucket { get; set; }
    public bool Uploaded { get; set; } = false;
    public Guid AssetId { get; set; }
    public AssetType AssetType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
