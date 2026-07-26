using BiegusowoApi.Data;
using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Blobs.Service;
using BiegusowoApi.Domain.Dtos.ProfilePage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
    ApplicationDbContext dbContext,
    IBlobService blobService) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IBlobService _blobService = blobService;

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get the profile of a specific user by their ID.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<ProfilePageResponse>> GetProfile(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("me")]
    [EndpointDescription("Get the profile of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    public Task<ActionResult<ProfilePageResponse>> GetMyProfile() 
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPatch("me")]
    [EndpointDescription("Update the profile information of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<ProfilePageResponse>> UpdateUserInfo(
        [FromBody] JsonPatchDocument<UserPatchRequest> request)
    {
        throw new NotImplementedException();
    }
 
    [HttpPost("me/avatar/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadAvatarPresigned(
        [FromBody] PresignedUploadFile request)
    {
        var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");

        if (identityId is null)
            return Unauthorized();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityId);

        //TODO standardize getting user from claims
        throw new NotImplementedException();
    }

    [HttpPut("me/avatar")]
    [EndpointDescription("Confirm the upload of an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmAvatarUpload(
        [FromBody] string key)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("me/avatar")]
    [EndpointDescription("Delete the avatar of the currently authenticated user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvatar()
    {
        throw new NotImplementedException();
    }

    [HttpPost("me/background-image/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadBackgroundImagePresigned(
        [FromBody] PresignedUploadFile request)
    {
        throw new NotImplementedException();
    }

    [HttpPut("me/background-image")]
    [EndpointDescription("Confirm the upload of a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBackgroundImageUpload(
        [FromBody] string key)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("me/background-image")]
    [EndpointDescription("Delete the background image of the currently authenticated user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBackgroundImage()
    {
        throw new NotImplementedException();
    }
}
