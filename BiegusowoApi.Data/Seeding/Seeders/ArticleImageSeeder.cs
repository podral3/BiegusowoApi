using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ArticleImageSeeder
{
    public static List<ArticleImage> Generate(List<Article> articles, int maxPhotosPerArticle)
    {
        var articleImages = new List<ArticleImage>();
        var random = new Random(42);
        int idCounter = 1;

        foreach (var article in articles)
        {
            int photoCount = random.Next(1, maxPhotosPerArticle + 1);

            for (int i = 0; i < photoCount; i++)
            {
                var image = new ArticleImage
                {
                    Id = DataSeeder.SeedGuid(idCounter++),
                    ArticleId = article.Id,
                    FileName = $"photo_{i + 1}.webp",
                    Order = (short)i,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                articleImages.Add(image);
            }
        }
        return articleImages;
    }
}