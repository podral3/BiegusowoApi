namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public record ProfilePageListingSummary(
    Guid Id,
    string Slug,
    string Title,
    decimal Price,
    string City,
    string? CoverPhotoUrl);
