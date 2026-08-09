namespace BiegusowoApi.Domain.Dtos.Auth;

public class AuthDtos
{
    public record ResendVerificationEmailRequest(string Email);
    public record PasswordResetRequest(string Email);
}
