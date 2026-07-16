namespace BiegusowoApi.Domain.Dtos.User;

public record UserDto(
    string username,
    string? description,
    string slug,
    string? phoneNumber,
    string displayName,
    string? city,
    DateTimeOffset joinDate);
