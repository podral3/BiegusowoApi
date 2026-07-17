using BiegusowoApi.Data.Types;
using NetTopologySuite.Geometries;

namespace BiegusowoApi.Data.Models;

public class Listing
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int SpeciesId { get; set; }
    public Species Species { get; set; } = null!;

    public int BreedId { get; set; }
    public Breed Breed { get; set; } = null!;

    public string Title { get; set; }
    public string Description { get; set; }
    public ListingType ListingType { get; set; }   
    public ListingStatus ListingStatus { get; set; }
    public string? BreedNote { get; set; }
    public double Price { get; set; }
    public bool PriceNegotiable { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? RemovedReason { get; set; }

    public int VoivodeshipId { get; set; }
    public Voivodeship Voivodeship { get; set; } = null!;
    public string CityName { get; set; }

    public Point Location { get; set; }

}