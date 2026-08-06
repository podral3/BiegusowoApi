using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal class BlobSeeder
{
    public static List<Blob> Generate(int count, string prefix, ref int idCounter)
    {
        var blobs = new List<Blob>();

        for (int i = 0; i < count; i++)
        {
            int number = idCounter++;
            blobs.Add(new Blob
            {
                StorageKey = $"{prefix}s/image{number}.webp",
                ContentType = "image/webp",
                Bucket = "public",
                Uploaded = true,
                CreatedAt = DateTimeOffset.UtcNow,
                DeletedAt = null
            });
        }

        return blobs;
    }
}
