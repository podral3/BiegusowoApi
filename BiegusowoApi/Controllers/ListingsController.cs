using BiegusowoApi.Domain.Dtos.Listing;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ListingsController : ControllerBase
{
    [HttpGet("id")]
    public ActionResult<ListingDto> GetListing(string id)
    {
        throw new NotImplementedException();
    }
}
