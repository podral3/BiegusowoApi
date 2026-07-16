using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Domain.Dtos.Misc;

public record SpeciesDto(
    int Id,
    string Name,
    string Slug)
{
    public SpeciesDto(Species species)
        : this(species.Id, species.Name, species.Slug)
    {
    }
}