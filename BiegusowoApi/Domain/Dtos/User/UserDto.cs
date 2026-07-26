namespace BiegusowoApi.Domain.Dtos.User;

public record UserDto(
    string Username,
    string? Description,
    string Slug,
    string? PhoneNumber,
    string DisplayName,
    string? City,
    DateTimeOffset JoinDate);


