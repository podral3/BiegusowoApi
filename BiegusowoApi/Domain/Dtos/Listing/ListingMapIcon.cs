namespace BiegusowoApi.Domain.Dtos.Listing;

public record ListingMapIcon(
    string Id,
    string Title,
    string Description,
    double Price,
    bool PriceNegotiable,
    string CityName,
    double Latitude,
    double Longitude);