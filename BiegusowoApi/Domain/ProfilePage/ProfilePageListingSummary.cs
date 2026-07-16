namespace BiegusowoApi.Domain.ProfilePage;

public record ProfilePageListingSummary(
    string Id,
    string Slug,
    string Title,
    decimal Price,
    string City,
    string? CoverPhotoUrl);
