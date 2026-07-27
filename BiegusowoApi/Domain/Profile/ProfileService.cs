using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Dtos.ProfilePage;
using BiegusowoApi.Domain.Dtos.User;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BiegusowoApi.Domain.Profile;

public class ProfileService(ApplicationDbContext db) : IProfileService
{
    private const int ListingPageSize = 12;

    private readonly ApplicationDbContext _dbContext = db;

    public async Task<ProfilePageResponse?> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null, ct);

        if (user is null)
            return null;

        return await BuildProfileResponseAsync(user, ct);
    }

    public Task<ProfilePageResponse?> GetMyProfileAsync(Guid currentUserId, CancellationToken ct = default)
        => GetProfileAsync(currentUserId, ct);

    public async Task<ProfilePageResponse?> UpdateMyProfileAsync(
        Guid currentUserId,
        JsonPatchDocument<UserPatchRequest> patch,
        CancellationToken ct = default)
    {
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId && u.DeletedAt == null, ct);

        if (user is null)
            return null;

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

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" | ", errors));
        }

        patch.ApplyTo(dto);

        user.DisplayName = dto.DisplayName;
        user.Bio = dto.Bio;
        user.PhoneNumber = dto.PhoneNumber;
        user.City = dto.City;
        user.VoivodeshipId = dto.VoivodeshipId;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        return await BuildProfileResponseAsync(user, ct);
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
            user.AvatarFileName,
            user.BackgroundFileName,
            [.. page.Select(MapListingSummary)],
            hasMoreListings);
    }

    private ProfilePageListingSummary MapListingSummary(Listing listing)
    {
        var images = JsonSerializer.Deserialize<Dictionary<string, string>>(listing.Images.RootElement.GetRawText());
        return new ProfilePageListingSummary(
            listing.Id,
            Slugify(listing.Title), // Replace with your real slug if you already store one.
            listing.Title,
            Convert.ToDecimal(listing.Price),
            listing.CityName,
            images["0"]);
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
