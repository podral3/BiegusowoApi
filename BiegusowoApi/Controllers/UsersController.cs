using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Blobs.Service;
using BiegusowoApi.Domain.Dtos.ProfilePage;
using BiegusowoApi.Domain.Profile;
using BiegusowoApi.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
    IProfileService profileService,
    IBlobService blobService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;
    private readonly IBlobService _blobService = blobService;

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get the profile of a specific user by their ID.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfilePageResponse>> GetProfile(Guid id)
    {
        var result = await _profileService.GetProfileAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    [EndpointDescription("Get the profile of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfilePageResponse>> GetMyProfile() 
    {
        var userId = User.GetUserId();
        var result = await _profileService.GetMyProfileAsync(userId);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpPatch("me")]
    [EndpointDescription("Update the profile information of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProfilePageResponse>> UpdateUserInfo(
        [FromBody] JsonPatchDocument<UserPatchRequest> request)
    {
        var userId = User.GetUserId();

        try
        {
            var result = await _profileService.UpdateMyProfileAsync(userId, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Microsoft.AspNetCore.JsonPatch.Exceptions.JsonPatchException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
 
    [HttpPost("me/avatar/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadAvatarPresigned(
        [FromBody] PresignedUploadFile request)
    {
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
