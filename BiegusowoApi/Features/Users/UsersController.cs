using Ardalis.Result.AspNetCore;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Blobs;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Features.Users.Dtos;
using BiegusowoApi.Shared.Helpers.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Features.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
    ApplicationDbContext dbContext,
    IProfileService profileService,
    IBlobService blobService) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
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
            return result.ToActionResult(this);
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

    [Authorize]
    [HttpPost("me/avatar/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadAvatarPresigned(
        [FromBody] PresignedUploadFile request)
    {
        PresignedUploadRequest parsed = new([request]);
        PresignedUploadResponse result = await _blobService.CreatePresignedUploadsAsync(parsed);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("me/avatar")]
    [EndpointDescription("Confirm the upload of an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmAvatarUpload(
        [FromBody] Guid blobId)
    {
        Guid userId = User.GetUserId();
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found");

        Dictionary<int, Guid> keys = new() { { 0, blobId } };
        ConfirmUploadRequest confirmRequest = new(keys);
        ConfirmUploadResult result = await _blobService.ConfirmUploadsAsync(confirmRequest);
        user.AvatarFileName = result.Result.FirstOrDefault()?.Key ?? user.AvatarFileName;
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("me/avatar")]
    [EndpointDescription("Delete the avatar of the currently authenticated user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvatar()
    {
        Guid userId = User.GetUserId();
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found");
        user.AvatarFileName = string.Empty;
        return Ok();
    }
    
    [Authorize] 
    [HttpPost("me/background-image/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadBackgroundImagePresigned(
        [FromBody] PresignedUploadFile request)
    {
        PresignedUploadRequest parsed = new([request]);
        PresignedUploadResponse result = await _blobService.CreatePresignedUploadsAsync(parsed);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("me/background-image")]
    [EndpointDescription("Confirm the upload of a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBackgroundImageUpload(
        [FromBody] Guid blobId)
    {
        Guid userId = User.GetUserId();
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found");

        Dictionary<int, Guid> keys = new() { { 0, blobId } };
        ConfirmUploadRequest confirmRequest = new(keys);
        ConfirmUploadResult result = await _blobService.ConfirmUploadsAsync(confirmRequest);
        user.BackgroundFileName = result.Result.FirstOrDefault()?.Key ?? user.BackgroundFileName;
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("me/background-image")]
    [EndpointDescription("Delete the background image of the currently authenticated user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBackgroundImage()
    {
        Guid userId = User.GetUserId();
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found");
        user.BackgroundFileName = string.Empty;
        return Ok();
    }
}
