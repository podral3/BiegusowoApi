using BiegusowoApi.Data;
using BiegusowoApi.Domain.Dtos.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    [HttpGet]
    public async Task<ActionResult<LookupDataDto>> GetLookups()
    {
        throw new NotImplementedException();    
    }
}
