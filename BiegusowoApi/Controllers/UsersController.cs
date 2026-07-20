using BiegusowoApi.Domain.Dtos.ProfilePage;
using BiegusowoApi.Domain.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BiegusowoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
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

    [Authorize]
    [HttpPatch("me/avatar")]
    [EndpointDescription("Update the avatar of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<ProfilePageResponse>> UpdateAvatar(
        [FromBody] string avatarUrl)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPatch("me/background-image")]
    [EndpointDescription("Update the background image of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<ProfilePageResponse>> UpdateBackgroundImage(
        [FromBody] string backgroundImageUrl)
    {
        throw new NotImplementedException();
    }

 
    [HttpPost("me/avatar/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadAvatarPresigned(
        [FromBody] PresignedUploadRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPut("me/avatar")]
    [EndpointDescription("Confirm the upload of an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmAvatarUpload(
        [FromBody] List<string> keys)
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
        [FromBody] PresignedUploadRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPut("me/background-image")]
    [EndpointDescription("Confirm the upload of a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBackgroundImageUpload(
        [FromBody] List<string> keys)
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
