using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
using Bogus;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;
namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ListingSeeder
{
    private static readonly DateTimeOffset SeedBaseTime =
       new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <param name="SpeciesBreedsRanges">Dict where each key is a species ID and the value is a tuple of (minBreedId, maxBreedId)</param>
    /// <returns></returns>
    public static (List<Listing> Listings, List<Blob> Blobs) Generate(int count, List<Breed> breeds, List<User> users, List<Voivodeship> voivodeships)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Breed temporaryBreed = null!;
        int idCounter = 1;
        int blobIdCounter = 1;
        var blobs = new List<Blob>();

        var listingFaker = new Bogus.Faker<Listing>()
            .UseSeed(42)
            .RuleFor(l => l.Id, f => DataSeeder.SeedGuid(idCounter++))
            .RuleFor(l => l.Title, f => $"Listing number {idCounter-1}")
            .RuleFor(l => l.Description, f => f.Lorem.Sentence(3, 5))
            .RuleFor(l => l.ListingType, f => f.PickRandom<ListingType>())
            .RuleFor(l => l.ListingStatus, f => f.PickRandom(
                Enumerable.Repeat(ListingStatus.Active, 8)   // weight Active heavily so tests have enough live listings
                .Concat(Enumerable.Repeat(ListingStatus.Sold, 2))
                .Concat(Enumerable.Repeat(ListingStatus.Expired, 1))
                .Concat(Enumerable.Repeat(ListingStatus.Removed, 1))
                .ToArray()))
            .RuleFor(l => l.UserId, f => f.PickRandom(users).Id)
            .RuleFor(l => l.BreedId, f =>
            {
                temporaryBreed = f.PickRandom(breeds); //First we pick a random breed, then we use its ID for the BreedId property and its SpeciesId for the SpeciesId property
                return temporaryBreed.Id;
            })
            .RuleFor(l => l.SpeciesId, f => temporaryBreed.SpeciesId)
            .RuleFor(l => l.BreedNote, f => f.Lorem.Sentence(3).OrNull(f, 0.3f))
            .RuleFor(l => l.Price, f => Math.Round(f.Random.Double(10, 200),2))
            .RuleFor(l => l.PriceNegotiable, f => f.Random.Bool(0.5f))
            .RuleFor(l => l.CreatedAt, f => SeedBaseTime.AddMinutes(-f.Random.Int(0, 30 * 24 * 60)))
            .RuleFor(l => l.UpdatedAt, (f, l) =>
            {
                if (f.Random.Float() < 0.4f)
                    return null;

                return l.CreatedAt.AddMinutes(f.Random.Int(1, 60 * 24 * 7));
            })
            .RuleFor(l => l.DeletedAt, (f, l) =>
            {
                if (l.ListingStatus == ListingStatus.Active)
                    return null;

                if (l.ListingStatus == ListingStatus.Removed)
                {
                    var baseTime = l.UpdatedAt ?? l.CreatedAt;
                    return baseTime.AddMinutes(f.Random.Int(1, 60 * 24 * 7));
                }

                return null;
            })
            .RuleFor(l => l.DeletedAt, (f, l) =>
            {
                // Active listings are never deleted
                if (l.ListingStatus == ListingStatus.Active)
                    return null;

                // Removed listings are always soft-deleted
                if (l.ListingStatus == ListingStatus.Removed)
                {
                    var baseTime = l.UpdatedAt ?? l.CreatedAt;
                    return baseTime.AddMinutes(f.Random.Int(1, 60 * 24 * 7));
                }
                return null;
            })
            .RuleFor(l => l.CityName, f => f.Address.City())
            .RuleFor(l => l.VoivodeshipId, f => f.PickRandom(voivodeships).Id)
            .RuleFor(l => l.Location, f =>
            {
                // Lat/Lon w przybliżonych granicach Polski
                var lat = f.Random.Double(49.0, 54.8);   // Y
                var lon = f.Random.Double(14.1, 24.2);   // X

                return geometryFactory.CreatePoint(new Coordinate(lon, lat));
            })
            .RuleFor(l => l.Images, f =>
            {
                int imageCount = f.Random.Int(2, 5);
                var imgBlobs = BlobSeeder.Generate(imageCount, "listing", ref blobIdCounter);
                blobs.AddRange(imgBlobs);

                var array = imgBlobs.Select(b => b.StorageKey).ToArray();
                return JsonSerializer.SerializeToDocument(array);
            });
        return (listingFaker.Generate(count), blobs);
    }
}
