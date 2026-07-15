using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoivodeshipsController(ApplicationDbContext dbContext) : ControllerBase
{
    private static List<Voivodeship>? _cachedVoivodeships;
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<Voivodeship>> GetVoivodeships()
    {
        _cachedVoivodeships ??= await _dbContext.Voivodeships
                .ToListAsync();
        return Ok(_cachedVoivodeships);
    }
}
