using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Domain.Dtos.Listing;

public sealed record UpdateListingRequest
(
    string? Title,
    string? Description,
    ListingType? ListingType,
    ListingStatus? ListingStatus,
    string? BreedNote,
    double? Price,
    bool? PriceNegotiable,
    int? SpeciesId,
    int? BreedId,
    int? VoivodeshipId,
    string? CityName,
    ImagePatchRequest? Images);

public sealed record ImagePatchRequest(
    IReadOnlyCollection<string>? Add,
    IReadOnlyCollection<string>? Remove);
