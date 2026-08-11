using Ardalis.Result;

namespace BiegusowoApi.Features.Auth;

public interface IAccountService
{
    Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result> ResendVerificationEmailAsync(string email, CancellationToken ct = default);
    Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default);
    Task DeleteKeycloakUserAsync(Guid userId, CancellationToken ct);

}
