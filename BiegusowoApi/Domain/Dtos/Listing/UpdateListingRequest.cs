using BiegusowoApi.Data.Types;

namespace BiegusowoApi.Domain.Dtos.Listing;

public class UpdateListingRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ListingType ListingType { get; set; }
    public ListingStatus ListingStatus { get; set; }
    public int SpeciesId { get; set; }
    public int BreedId { get; set; }
    public string? BreedNote { get; set; }
    public double Price { get; set; }
    public bool PriceNegotiable { get; set; }
    public int VoivodeshipId { get; set; }
    public string CityName { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public sealed record ImagePatchRequest(
    IReadOnlyCollection<string>? Add,
    IReadOnlyCollection<string>? Remove);
