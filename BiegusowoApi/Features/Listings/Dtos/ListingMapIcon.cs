namespace BiegusowoApi.Features.Listings.Dtos;

public record ListingMapIcon(
    Guid Id,
    string Title,
    int BreedId,
    double Price,
    double Latitude,
    double Longitude);