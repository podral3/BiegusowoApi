using BiegusowoApi.Data.Models;
using Bogus;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ArticleSeeder
{
    private static readonly string[] Statuses = ["Draft", "Published", "Archived"];
    private static readonly DateTimeOffset SeedBaseTime =
       new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero);

    public static (List<Article> Articles, List<Blob> Blobs) Generate(int count)
    {
        int idCounter = 1;
        int blobIdCounter = 1;
        var blobs = new List<Blob>();

        var articleFaker = new Faker<Article>()
            .UseSeed(42)
            .RuleFor(a => a.Id, f => DataSeeder.SeedGuid(idCounter++))
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(4, 4))
            .RuleFor(a => a.Slug, (f, a) => f.Lorem.Slug())
            .RuleFor(a => a.Excerpt, f => f.Lorem.Sentence(10, 10))
            .RuleFor(a => a.CoverURL, f => f.Image.PicsumUrl())
            .RuleFor(a => a.BodyHTML, f => $"<p>{f.Lorem.Paragraphs(5, "</p><p>")}</p>")
            .RuleFor(a => a.MetaTitle, (f, a) => a.Title)
            .RuleFor(a => a.MetaDescription, f => f.Lorem.Sentence(15, 10))
            .RuleFor(a => a.OgImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(a => a.Status, f => f.PickRandom(Statuses)) //TODO fix
            .RuleFor(a => a.ReadingTimeMinutes, f => f.Random.Int(1, 15))
            .RuleFor(a => a.PublishedAt, f => SeedBaseTime.AddMinutes(-f.Random.Int(0, 15 * 24 * 60)))
            .RuleFor(l => l.Images, f =>
            {
                int imageCount = f.Random.Int(2, 5);
                var imgBlobs = BlobSeeder.Generate(imageCount, "article", ref blobIdCounter);
                blobs.AddRange(imgBlobs);

                var array = imgBlobs.Select(b => b.StorageKey).ToArray();
                return JsonSerializer.SerializeToDocument(array);
            });

        var articles = articleFaker.Generate(count);

      
        return (articles, blobs);
    }
}