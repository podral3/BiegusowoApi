using BiegusowoApi.Data.Models;
using Bogus;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ArticleSeeder
{
    private static readonly string[] Statuses = ["Draft", "Published", "Archived"];

    public static (List<Article> Articles, List<Blob> Blobs) Generate(int count)
    {
        int idCounter = 1;
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
            .RuleFor(a => a.PublishedAt, f => f.Date.RecentOffset(60).ToUniversalTime())
            .RuleFor(a => a.Images, f =>
            {
                int imageCount = f.Random.Int(3, 5);
                var imgBlobs = BlobSeeder.Generate(imageCount, "article");
                blobs.AddRange(imgBlobs);

                var dict = imgBlobs
                    .Select((b, i) => new { Order = (i).ToString(), b.FileName })
                    .ToDictionary(x => x.Order, x => x.FileName);

                return JsonSerializer.SerializeToDocument(dict);
            });

        var articles = articleFaker.Generate(count);

      
        return (articles, blobs);
    }
}