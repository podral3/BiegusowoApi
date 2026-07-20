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
    [EndpointDescription("Get lookups for species, breeds, and voivodeships and their corresponding IDs.")]
    [ProducesResponseType(typeof(LookupDataDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LookupDataDto>> GetLookups()
    {
        throw new NotImplementedException();    
    }
}
