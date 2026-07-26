using System.ComponentModel.DataAnnotations;

namespace BiegusowoApi.Options;

public class ImageValidationOptions
{
    [Required] public long MaxFileSizeBytes { get; set; }

    [Required] public HashSet<string> AllowedContentTypes { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

}
