using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Domain.Dtos.Listing;

public record ListingDto(
    string Id,
    string UserId,
    string UserName,
    string Title,
    string Description,
    ListingType ListingType,
    ListingStatus ListingStatus,
    string BreedNote,
    double Price,
    bool PriceNegotiable,
    string SpeciesId,
    string BreedId,
    List<string> ImageUrls,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int VoivodeshipId,
    string CityName);