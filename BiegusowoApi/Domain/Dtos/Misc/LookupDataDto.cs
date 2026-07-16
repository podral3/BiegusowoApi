using BiegusowoApi.Data.Models;

namespace BiegusowoApi.Domain.Dtos.Misc;

public record LookupDataDto(
    IReadOnlyList<SpeciesDto> Species,
    IReadOnlyList<BreedDto> Breeds,
    IReadOnlyList<Voivodeship> Voivodeships);
