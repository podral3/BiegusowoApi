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
        foreach (var listing in listings)
        { 
        int listingPhotoCount = new Random(42).Next(maxPhotosPerListing);


            for (int i = 0; i < listingPhotoCount; i++)
            {
                var photo = new ListingImage
                {
                    Id = Guid.NewGuid(),
                    ListingId = listing.Id,
                    FileName = $"photo_{i + 1}.webp",
                    SortOrder = i,
                    Bucket = "public",
                    FileSizeBytes = new Random().Next(100_000, 5_000_000), // Random file size between 100KB and 5MB
                    CreatedAt = DateTimeOffset.UtcNow
                };
                listingPhotos.Add(photo);
            }
        }
        return listingPhotos;
    }
}
