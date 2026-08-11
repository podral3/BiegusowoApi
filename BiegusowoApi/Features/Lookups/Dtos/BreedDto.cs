using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Features.Lookups.Dtos;

public record BreedDto(
    int Id,
    int SpeciesId,
    string Name,
    string Slug)
{
    public BreedDto(Breed breed)
        : this(breed.Id, breed.SpeciesId, breed.Name, breed.Slug)
    {
    }
}