using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Features.Listings.Dtos;

public record ListingDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string Title,
    string Description,
    ListingType ListingType,
    ListingStatus ListingStatus,
    string BreedNote,
    double Price,
    bool PriceNegotiable,
    int SpeciesId,
    int BreedId,
    Dictionary<string, string> ImageOrderedUrls,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int VoivodeshipId,
    string CityName);