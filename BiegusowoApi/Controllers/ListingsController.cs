using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Helpers;
using BiegusowoApi.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ListingsController : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Get a paginated list of listings with optional filters and sorting.")]
    [ProducesResponseType(typeof(CursorPaginatedList<MinimalListingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPaginatedList<MinimalListingDto>>> GetListings(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? sort,
        [FromQuery] int? speciesId,
        [FromQuery] int? breedId,
        [FromQuery] int? priceMin,
        [FromQuery] int? priceMax,
        [FromQuery] string? beforeCursorValue, 
        [FromQuery] Guid? beforeListingId,    
        [FromQuery] int pageSize = 20)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get a specific listing by its ID.")]
    [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ListingDto> GetListing([FromRoute] Guid id)
    {
        Guid userId = User.GetUserId();
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost]
    [EndpointDescription("Create a new listing.")]
    [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListingDto>> CreateListing([FromBody] CreateListingRequest request)
    {
        throw new NotImplementedException();    
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    [EndpointDescription("Update an existing listing.")]
    [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListingDto>> PatchListing(
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument<UpdateListingRequest> request)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [EndpointDescription("Delete a specific listing.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]   
    public async Task<ActionResult> DeleteListing([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("{id:guid}/images/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading images associated with a specific listing.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadPresigned(
        [FromRoute] Guid id,
        [FromBody] PresignedUploadRequest request)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("{id:guid}/images")]
    [EndpointDescription("Confirm the upload of images associated with a specific listing.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmUpload(
        [FromRoute] Guid id,
        [FromBody] List<string> keys)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpDelete("{id:guid}/images/")]
    [EndpointDescription("Delete an image associated with a specific listing.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImages(
        [FromRoute] Guid id,
        [FromBody] List<string> imageIds)
    {
        throw new NotImplementedException();
    }
}
