using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpeciesController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<List<SpeciesDto>>> GetSpecies()
    {
        List<SpeciesDto> species = await _dbContext.Species.Select(s => new SpeciesDto(s))
            .ToListAsync();
        return Ok(species);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SpeciesDto>> CreateSpecies([FromBody] string name)
    {
        var species = new Species
        {
            Name = name,
            Slug = name.ToLower().Replace(" ", "-") //Todo sanitize slug
        };

        _dbContext.Species.Add(species);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSpecies), new { id = species.Id }, new SpeciesDto(species));
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSpecies(int id)
    {
        var species = await _dbContext.Species.FindAsync(id);
        if (species == null)
        {
            return NotFound();
        }

        _dbContext.Species.Remove(species);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
