using BiegusowoApi.Data.Models;
using Bogus;
using Bogus.Extensions;
using System.Diagnostics.Metrics;
using static System.Reflection.Metadata.BlobBuilder;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class UserSeeder
{
    public static (List<User> Users, List<Blob> Blobs) Generate(int count, List<Voivodeship> voivodeships)
    {
        int idCounter = 1;
        int blobIdCounter = 1;
        var blobs = new List<Blob>();
        var userFaker = new Faker<User>()
            .UseSeed(42)
            .RuleFor(l => l.Id, f => DataSeeder.SeedGuid(idCounter++))
            .RuleFor(u => u.DisplayName, f => f.Internet.UserName())
            .RuleFor(u => u.Bio, f => f.Lorem.Paragraph().OrNull(f, 0.2f))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber().OrNull(f, 0.2f))
            .RuleFor(u => u.City, f => f.Address.City())
            .RuleFor(u => u.IsOnboarded, f => true)
            .RuleFor(u => u.VoivodeshipId, f => f.PickRandom(voivodeships).Id)
            .RuleFor(u => u.AvatarFileName, f =>
            {
                var blob = BlobSeeder.Generate(1, "avatar", ref blobIdCounter)[0];
                blobs.Add(blob);
                return blob.StorageKey;
            })
            .RuleFor(u => u.BackgroundFileName, f =>
            {
                var blob = BlobSeeder.Generate(1, "background", ref blobIdCounter)[0];
                blobs.Add(blob);
                return blob.StorageKey;
            });

        var users = userFaker.Generate(count);
        return (users, blobs);
    }
}
