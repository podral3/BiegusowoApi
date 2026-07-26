using System.ComponentModel.DataAnnotations;

namespace BiegusowoApi.Domain.Blobs;

public class FileStorageOptions
{
    [Required] public int MaxWidth { get; set; } = 1920;
    [Required] public int MaxHeight { get; set; } = 1080;
    [Required] public long MaxFileSizeInBytes { get; set; } = 5242880; // 5 MB
    [Required] public string AllowedFileExtensions { get; set; } = ".webp";
    [Required] public string Bucket { get; set; } = "public";
}