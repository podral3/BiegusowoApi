using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Features.Lookups.Dtos;

public record LookupDataDto(
    IReadOnlyList<SpeciesDto> Species,
    IReadOnlyList<BreedDto> Breeds,
    IReadOnlyList<Voivodeship> Voivodeships);
