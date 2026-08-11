using BiegusowoApi.Data;
using BiegusowoApi.Features.Lookups.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Features.Lookups;

[Route("api/[controller]")]
[ApiController]
public class LookupsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    [HttpGet]
    [EndpointDescription("Get lookups for species, breeds, and voivodeships and their corresponding IDs.")]
    [ProducesResponseType(typeof(LookupDataDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LookupDataDto>> GetLookups()
    {
        var breeds = (await _dbContext.Breeds.ToListAsync())
            .Select(b => new BreedDto(b))
            .ToList();
        var species = (await _dbContext.Species.ToListAsync())
            .Select(s => new SpeciesDto(s))
            .ToList();
        var voivodeships = await _dbContext.Voivodeships.ToListAsync();

        return Ok(new LookupDataDto(species, breeds, voivodeships));
    }
}
