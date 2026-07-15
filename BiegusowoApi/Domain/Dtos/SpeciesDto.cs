using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Domain.Dtos;

public class SpeciesDto(Species species)
{
    public int Id { get; set; } = species.Id;
    public string Name { get; set; } = species.Name;
    public string Slug { get; set; } = species.Slug;
}
