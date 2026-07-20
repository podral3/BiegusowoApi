namespace BiegusowoApi.Domain.Dtos.Listing;

public record ListingMapIcon(
    Guid Id,
    string Title,
    int BreedId,
    double Price,
    double Latitude,
    double Longitude);