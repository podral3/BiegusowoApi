using Ardalis.Result.AspNetCore;
using Ardalis.Result.FluentValidation;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Accounts.Dtos;
using BiegusowoApi.Features.Auth;
using BiegusowoApi.Features.Blobs;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Features.Users.Dtos;
using BiegusowoApi.Shared.Helpers.Claims;
using BiegusowoApi.Shared.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BiegusowoApi.Features.Users;

[Route("api/[controller]")]
[ApiController]
public class ProfilesController(
    ApplicationDbContext dbContext,
    IProfileService profileService,
    IBlobService blobService,
    IOptions<SupabaseJwtOptions> options,
    ILogger<ProfilesController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IProfileService _profileService = profileService;
    private readonly IBlobService _blobService = blobService;
    private readonly IOptions<SupabaseJwtOptions> _supabaseOptions = options;
    private readonly ILogger<ProfilesController> _logger = logger;

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get the profile of a specific user by their ID.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfilePageResponse>> GetProfile(Guid id)
    {
        var result = await _profileService.GetProfileAsync(id);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("me")]
    [EndpointDescription("Get the profile of the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfilePageResponse>> GetMyProfile() 
    {
        var userId = User.GetUserId();

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == userId);

        if (user is null)
        {
            return Ok(new
            {
                authenticated = true,
                onboardingRequired = true
            });
        }

        var result = await _profileService.GetMyProfileAsync(userId);
        return result.ToActionResult(this);
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

        var result = await _profileService.UpdateMyProfileAsync(userId, request);
        return result.ToActionResult(this);        
    }

    [HttpPost("supabase/user-created")]
    public async Task<ActionResult<Guid>> HandleUserCreated(CancellationToken cancellationToken)
    {
        var expectedSecret = _supabaseOptions.Value.WebhookSecret ;
        var authHeader = Request.Headers.Authorization.ToString();
        bool isAuthorized = !string.IsNullOrEmpty(expectedSecret) && authHeader == $"Bearer {expectedSecret}";

        if (!isAuthorized)
        {
            return Unauthorized();
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        SupabaseUserCreatedPayload? payload;
        using (var reader = new StreamReader(Request.Body))
        {
            var body = await reader.ReadToEndAsync(cancellationToken);
            payload = JsonSerializer.Deserialize<SupabaseUserCreatedPayload>(body, options);
        }

        if (payload?.Record is null || payload.Record.Id == Guid.Empty)
        {
            return BadRequest("Missing user record in payload.");
        }

        var result = await _profileService.CreatePendingUserAsync(
            payload.Record.Id,
            payload.Record.CreatedAt ?? DateTimeOffset.UtcNow,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPost("onboarding")]
    [EndpointDescription("Complete profile setup for the currently authenticated user, created earlier via Supabase webhook.")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> CompleteOnboarding(
    [FromBody] OnboardingRequest request,
    CancellationToken cancellationToken)
    {
        OnboardingRequestValidator validator = new();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.AsErrors());
        }

        var userId = User.GetUserId();

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            // Webhook hasn't landed yet, or something went wrong upstream.
            _logger.LogWarning("Onboarding attempted for {UserId} but no user record exists.", userId);
            return NotFound(new { code = "user_not_found" });
        }

        if (user.IsOnboarded)
        {
            _logger.LogWarning("User {UserId} attempted to onboard again.", userId);
            return Conflict(new { code = "account_already_setup" });
        }

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;
        user.PhoneNumber = request.PhoneNumber;
        user.City = request.CityName;
        user.VoivodeshipId = request.VoivodeshipId;
        user.IsOnboarded = true;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new UserDto(user.DisplayName!, user.Bio, "Todo: Generate Slug", user.PhoneNumber, user.DisplayName!, user.City!, user.CreatedAt));
    }

    [Authorize]
    [HttpPost("me/avatar/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableRateLimiting("presigned-uploads")]
    public async Task<ActionResult<PresignedUploadResponse>> UploadAvatarPresigned(
        [FromBody] PresignedUploadFile request)
    {
        PresignedUploadRequest parsed = new([request]);
        var result = await _blobService.CreatePresignedUploadsAsync(parsed);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPut("me/avatar")]
    [EndpointDescription("Confirm the upload of an avatar associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EnableRateLimiting("presigned-uploads")]
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
        await _dbContext.SaveChangesAsync();
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
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize] 
    [HttpPost("me/background-image/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EnableRateLimiting("presigned-uploads")]
    public async Task<ActionResult<PresignedUploadResponse>> UploadBackgroundImagePresigned(
        [FromBody] PresignedUploadFile request)
    {
        PresignedUploadRequest parsed = new([request]);
        var result = await _blobService.CreatePresignedUploadsAsync(parsed);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPut("me/background-image")]
    [EndpointDescription("Confirm the upload of a background image associated with the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfilePageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EnableRateLimiting("presigned-uploads")]
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
        await _dbContext.SaveChangesAsync();
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
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found");
        user.BackgroundFileName = string.Empty;
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}
