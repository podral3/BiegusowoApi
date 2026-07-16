namespace BiegusowoApi.Domain.Dtos.Listing;

public record MinimalListingDto(
    string Id,
    string Title,
    double Price,
    string City,
    int SpeciedId,
    int BreedId,
    int TransactionTypeId,
    string ImageUrl,
    string Slug,
    double Latitude,
    double Longitude);

