using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Blobs.Service;
using BiegusowoApi.Domain.Dtos.Listing;
using BiegusowoApi.Domain.Listings;
using BiegusowoApi.Helpers;
using BiegusowoApi.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ListingsController(IListingService listingService,
    IBlobService blobService): ControllerBase
{
    private readonly IListingService _listingService = listingService;
    private readonly IBlobService _blobService = blobService;

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
        return await _listingService.GetListingsAsync(search, city, sort, speciesId, breedId, priceMin, priceMax, beforeCursorValue, beforeListingId, pageSize);
    }

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get a specific listing by its ID.")]
    [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListingDto>> GetListing([FromRoute] Guid id)
    {
        var result = await _listingService.GetListingAsync(id);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPost]
    [EndpointDescription("Create a new listing.")]
    [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListingDto>> CreateListing([FromBody] CreateListingRequest request)
    {
        Guid userId = User.GetUserId();
        var restult = await _listingService.CreateListingAsync(userId, request);
        return restult.ToActionResult(this);
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
        Guid userId = User.GetUserId();
        var result = await _listingService.PatchListingAsync(userId, id, request);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [EndpointDescription("Delete a specific listing.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]   
    public async Task<ActionResult> DeleteListing([FromRoute] Guid id)
    {
       var result = await _listingService.DeleteListingAsync(User.GetUserId(), id);
       return result.ToActionResult(this);
    }

    //[Authorize]
    [HttpPost("images/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading images associated with a specific listing.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadPresigned(
        [FromBody] PresignedUploadRequest request)
    {
        var result = await _blobService.CreatePresignedUploadsAsync(request);
        return Ok(result);
    }

    //[Authorize]
    [HttpPost("images")]
    [EndpointDescription("Confirm the upload of images associated with a specific listing.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmUpload(
        [FromBody] ConfirmUploadRequest request)
    {
        var result = await _blobService.ConfirmUploadsAsync(request);
        return Ok(result);
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
