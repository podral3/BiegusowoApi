using System.ComponentModel.DataAnnotations;

namespace BiegusowoApi.Shared.Options;

public class S3Options
{
    public const string AppsettingsSection = "S3";
    [Required] public string Provider { get; set; } = "R2";
    [Required] public string BucketName { get; set; } = null!;
    [Required] public string PublicBaseUrl { get; set; } = string.Empty;
    public string? PutEndpoint { get; set; }
    [Required] public string AccessKey { get; set; } = string.Empty;
    [Required] public string SecretKey { get; set; } = string.Empty;
    [Required] public string Region { get; set; } = string.Empty;
    [Required] public int UrlExpiry { get; set; } = 360;
}