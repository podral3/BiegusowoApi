using BiegusowoApi.Data.Models;
using Bogus;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiegusowoApi.Data.Seeding.Seeders;

internal static class ListingPhotoSeeder
{
    public static List<ListingImage> Generate(List<Listing> listings, int maxPhotosPerListing)
    {
        var listingPhotos = new List<ListingImage>();
        var random = new Random(42);
        int idCounter = 1;

        foreach (var listing in listings)
        {
            int listingPhotoCount = random.Next(maxPhotosPerListing);

            for (int i = 0; i < listingPhotoCount; i++)
            {
                var photo = new ListingImage
                {
                    Id = DataSeeder.SeedGuid(idCounter++),
                    ListingId = listing.Id,
                    FileName = $"photo_{i + 1}.webp",
                    SortOrder = i,
                    Bucket = "public",
                    FileSizeBytes = random.Next(100_000, 5_000_000),
                    CreatedAt = DateTimeOffset.UtcNow
                };
                listingPhotos.Add(photo);
            }
        }
        return listingPhotos;
    }
}
