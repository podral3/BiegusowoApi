using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BreedsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<List<BreedDto>>> GetBreeds()
    {
        List<BreedDto> breeds = await _dbContext.Breeds.Select(b => new BreedDto(b))
            .ToListAsync();
        return Ok(breeds);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BreedDto>> AddBreed([FromBody] int speciesId, [FromBody] string name)
    {
        var breed = new Breed
        {
            SpeciesId = speciesId,
            Name = name,
            Slug = name.ToLower().Replace(" ", "-") //Todo sanitize slug
        };

        _dbContext.Breeds.Add(breed);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBreeds), new { id = breed.Id }, new BreedDto(breed));
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBreed(int id)
    {
        var breed = await _dbContext.Breeds.FindAsync(id);
        if (breed == null)
        {
            return NotFound();
        }

        _dbContext.Breeds.Remove(breed);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
