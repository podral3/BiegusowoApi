using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
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

    [HttpGet("{id:string}")]
    public ActionResult<ListingDto> GetListing([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{id:string}")]
    public ActionResult<ListingDto> PatchListing([FromRoute] string id, [FromBody] UpdateListingRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:string}")]
    public ActionResult DeleteListing([FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}
