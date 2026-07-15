using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Domain.Dtos;

public class BreedDto(Breed breed)
{
    public int Id { get; set; } = breed.Id;
    public int SpeciesId { get; set; } = breed.SpeciesId;
    public string Name { get; set; } = breed.Name;
    public string Slug { get; set; } = breed.Slug;
}
