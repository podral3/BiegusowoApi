namespace BiegusowoApi.Features.Accounts.Dtos;

public sealed record SetupAccountRequest(
    string DisplayName,
    string? Bio,
    string? PhoneNumber,
    string? CityName,
    int VoivodeshipId);