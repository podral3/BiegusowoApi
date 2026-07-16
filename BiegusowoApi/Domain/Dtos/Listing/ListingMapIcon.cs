namespace BiegusowoApi.Domain.Dtos.Listing;

public record ListingMapIcon(
    string Id,
    string Title,
    int breedId,
    double Price,
    double Latitude,
    double Longitude);