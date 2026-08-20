using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Features.Accounts.Dtos;
using BiegusowoApi.Features.Auth;
using BiegusowoApi.Features.Users.Dtos;
using BiegusowoApi.Shared.Options;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BiegusowoApi.Features.Users;

public class ProfileService(
    ApplicationDbContext db,
    ILogger<ProfileService> logger,
    IOptions<SupabaseJwtOptions> supabaseOptions,
    IOptions<S3Options> fileStorageOptions) : IProfileService
{
    private const int ListingPageSize = 12;

    private readonly ApplicationDbContext _dbContext = db;
    private readonly ILogger<ProfileService> _logger = logger;
    private readonly IOptions<S3Options> _fileStorageOptions = fileStorageOptions;

    public async Task<Result<ProfilePageResponse>> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null, ct);

        if (user is null)
            return Result<ProfilePageResponse>.NotFound();

        var profileResponse = await BuildProfileResponseAsync(user, ct);
        return Result<ProfilePageResponse>.Success(profileResponse);
    }

    public Task<Result<ProfilePageResponse>> GetMyProfileAsync(Guid currentUserId, CancellationToken ct = default)
        => GetProfileAsync(currentUserId, ct);

    public async Task<Result<ProfilePageResponse>> UpdateMyProfileAsync(
        Guid currentUserId,
        JsonPatchDocument<UserPatchRequest> patch,
        CancellationToken ct = default)
    {
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId && u.DeletedAt == null, ct);

        if (user is null)
            return Result<ProfilePageResponse>.NotFound();

        var dto = new UserPatchRequest
        {
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            PhoneNumber = user.PhoneNumber,
            City = user.City,
            VoivodeshipId = user.VoivodeshipId
        };

        var errors = new List<string>();

        patch.ApplyTo(dto, err =>
        {
            errors.Add($"{err.AffectedObject.GetType().Name}: {err.ErrorMessage}");
        });

        UserPatchRequestValidator validator = new();
        var validation = await validator.ValidateAsync(dto, ct);

        if (!validation.IsValid)
        {
            return Result<ProfilePageResponse>.Invalid(validation.AsErrors());
        }

        user.DisplayName = dto.DisplayName;
        user.Bio = dto.Bio;
        user.PhoneNumber = dto.PhoneNumber;
        user.City = dto.City;
        user.VoivodeshipId = dto.VoivodeshipId;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        return Result<ProfilePageResponse>.Success(await BuildProfileResponseAsync(user, ct));
    }

    private async Task<ProfilePageResponse> BuildProfileResponseAsync(User user, CancellationToken ct)
    {
        var listingsQuery = _dbContext.Listings
            .AsNoTracking()
            .Where(l => l.UserId == user.Id && l.DeletedAt == null)
            .OrderByDescending(l => l.CreatedAt);

        var listings = await listingsQuery
            .Take(ListingPageSize + 1)
            .ToListAsync(ct);

        var hasMoreListings = listings.Count > ListingPageSize;
        var page = listings.Take(ListingPageSize).ToList();

        var userDto = MapUserDto(user);

        return new ProfilePageResponse(
            userDto,
            $"{_fileStorageOptions.Value.PublicBaseUrl.TrimEnd('/')}/{user.AvatarFileName?.TrimStart('/')}",
            $"{_fileStorageOptions.Value.PublicBaseUrl.TrimEnd('/')}/{user.BackgroundFileName?.TrimStart('/')}",
            [.. page.Select(MapListingSummary)],
            hasMoreListings);
    }

    public async Task<Result<Guid>> CreatePendingUserAsync(
    Guid userId,
    DateTimeOffset createdAt,
    CancellationToken ct = default)
    {
        var existingUser = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, ct);

        if (existingUser is not null)
        {
            _logger.LogInformation("User {UserId} already exists, skipping creation.", userId);
            return Result.Success();
        }

        var user = new User
        {
            Id = userId,
            IsOnboarded = false,
            CreatedAt = createdAt,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Created pending user record for {UserId}.", userId);

        return Result.Created(user.Id);
    }

    public async Task<Result<UserDto>> CompleteOnboarding(Guid userId, OnboardingRequest request, CancellationToken ct = default)
    {
        OnboardingRequestValidator validator = new();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Invalid(validation.AsErrors());
        }

        User? user = await _dbContext.Users
            .SingleOrDefaultAsync(
                x => x.Id == userId,
                ct);

        if (user is null)
        {
            // Webhook hasn't landed yet, or something went wrong upstream.
            _logger.LogWarning("Onboarding attempted for {UserId} but no user record exists.", userId);
            return Result<UserDto>.NotFound("user_not_found");
        }

        if (user.IsOnboarded)
        {
            _logger.LogWarning("User {UserId} attempted to onboard again.", userId);
            return Result<UserDto>.Conflict("account_already_setup");
        }

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;
        user.PhoneNumber = request.PhoneNumber;
        user.City = request.CityName;
        user.VoivodeshipId = request.VoivodeshipId;
        user.IsOnboarded = true;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        return Result<UserDto>.Success(new UserDto(user.DisplayName!, user.Bio, "Todo: Generate Slug", user.PhoneNumber, user.DisplayName!, user.City!, user.CreatedAt));
    }
    private ProfilePageListingSummary MapListingSummary(Listing listing)
    {
        var images = JsonSerializer.Deserialize<string[]>(listing.Images.RootElement.GetRawText());
        var firstImage = images is { Length: > 0 } ? images[0] : string.Empty;

        return new ProfilePageListingSummary(
            listing.Id,
            Slugify(listing.Title), // Replace with your real slug if you already store one.
            listing.Title,
            Convert.ToDecimal(listing.Price),
            listing.CityName,
            firstImage);
    }
    private static string Slugify(string value)
    {
        var s = value.Trim().ToLowerInvariant();
        s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\s-]", "");
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", "-");
        s = System.Text.RegularExpressions.Regex.Replace(s, @"-+", "-");
        return s.Trim('-');
    }

    private static UserDto MapUserDto(User user)
    {
        return new UserDto(user.DisplayName, user.Bio, user.DisplayName, user.PhoneNumber, user.DisplayName, user.City, user.CreatedAt);
    }
}
