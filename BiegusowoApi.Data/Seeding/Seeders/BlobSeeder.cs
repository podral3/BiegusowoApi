using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal class BlobSeeder
{
    private static int _idCounter = 1;

    public static List<Blob> Generate(int count, string prefix)
    {
        var blobs = new List<Blob>();

        for (int i = 0; i < count; i++)
        {
            int number = _idCounter++;
            blobs.Add(new Blob
            {
                Id = DataSeeder.SeedGuid(number),
                StorageKey = $"{prefix}/image{number}.webp",
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
