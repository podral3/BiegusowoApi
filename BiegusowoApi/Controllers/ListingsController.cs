using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ListingsController : ControllerBase
{
    [HttpGet]
    public ActionResult<PaginatedList<ListingDto>> GetListings(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? sort,
        [FromQuery] int? speciesId,
        [FromQuery] int? breedId,
        [FromQuery] int? priceMin,
        [FromQuery] int? priceMax,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public ActionResult<ListingDto> GetListing([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ListingDto>> CreateListing([FromBody] CreateListingRequest request)
    {
        throw new NotImplementedException();    
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<ActionResult<ListingDto>> PatchListing([FromRoute] string id, [FromBody] UpdateListingRequest request)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteListing([FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}
