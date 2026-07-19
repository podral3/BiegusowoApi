namespace BiegusowoApi.Domain.Dtos.Listing;

public record CreateListingRequest(
    string Title,
    string Description,
    double Price,
    bool PriceNegotiable,
    int SpeciesId,
    int BreedId,
    string? BreedNote,
    int VoivodeshipId,
    int ListingTypeId,
    int ListingStatusId,
    string CityName,
    double Longitude,
    double Latitude,
    List<string> ImageIds
);
