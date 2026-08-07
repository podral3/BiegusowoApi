namespace BiegusowoApi.Domain.Dtos.User;

public record RegisterRequest(
    string Email,
    string Password,
    string DisplayName,
    string? Bio,
    string City,
    int VoivodeshipId,
    string? PhoneNumber);

public record RegisterResult(Guid UserId);

public class RegistrationFailedException(string message) : Exception(message);