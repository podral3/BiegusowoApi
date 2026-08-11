namespace BiegusowoApi.Features.Auth;

public record UserDto(
    string Username,
    string? Bio,
    string Slug,
    string? PhoneNumber,
    string DisplayName,
    string? City,
    DateTimeOffset JoinDate);


