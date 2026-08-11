using Ardalis.Result;
using BiegusowoApi.Data;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using System.Net;

namespace BiegusowoApi.Features.Auth;

public class AccountService(
    IKeycloakUserClient keycloakUserClient,
    ApplicationDbContext db,
    ILogger<AccountService> logger) : IAccountService
{
    private const string Realm = "biegusowo";
    private const string FrontendClientId = "biegusowo-test";
    private const string AppRedirectUri = "https://biegusowo.gekonlab.org/me";

    public async Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var keycloakUser = new UserRepresentation
        {
            Username = request.Email,
            Email = request.Email,
            EmailVerified = false,
            Enabled = true,
            Credentials =
            [
                new CredentialRepresentation
                {
                    Type = "password",
                    Value = request.Password,
                    Temporary = false
                }
            ]
        };

        Guid keycloakUserId;
        try
        {
            var response = await keycloakUserClient.CreateUserWithResponseAsync(Realm, keycloakUser, ct);

            if (response.StatusCode == HttpStatusCode.Conflict)
                return Result<RegisterResult>.Conflict();

            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location
                ?? throw new InvalidOperationException("Keycloak returned no Location header.");
            keycloakUserId = Guid.Parse(location.Segments[^1]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Keycloak user for {Email}", request.Email);
            return Result<RegisterResult>.CriticalError();
        }

        
        await keycloakUserClient.SendVerifyEmailAsync(
            Realm, keycloakUserId.ToString(), FrontendClientId, AppRedirectUri, ct);
            

        var user = new Data.Models.User
        {
            Id = keycloakUserId,
            DisplayName = request.DisplayName,
            City = request.City,
            Bio = request.Bio,
            VoivodeshipId = request.VoivodeshipId,
            PhoneNumber = request.PhoneNumber,
            AvatarFileName = "default-avatar.png",
            BackgroundFileName = "default-background.png",
        };

        try
        {
            db.Users.Add(user);
            await db.SaveChangesAsync(ct);
        }
        catch
        {
            await DeleteKeycloakUserAsync(keycloakUserId, ct);
            throw;
        }

        return Result<RegisterResult>.Created(new RegisterResult(user.Id));
    }

    public async Task<Result> ResendVerificationEmailAsync(string email, CancellationToken ct = default)
    {
        var userId = await FindKeycloakUserIdByEmailAsync(email, ct);

        if (userId is null)
        {
            logger.LogInformation("Resend-verification requested for unknown email {Email}", email);
            return Result.NotFound();
        }

        await keycloakUserClient.SendVerifyEmailAsync(
            Realm, userId.Value.ToString(), FrontendClientId, AppRedirectUri, ct);

        return Result.Success();
    }

    public async Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default)
    {
        var userId = await FindKeycloakUserIdByEmailAsync(email, ct);

        if (userId is null)
        {
            logger.LogInformation("Password-reset requested for unknown email {Email}", email);
            return Result.NotFound();
        }

        ExecuteActionsEmailRequest request = new()
        {
            Actions = ["UPDATE_PASSWORD"],
            ClientId = FrontendClientId,
            RedirectUri = AppRedirectUri
        };

        await keycloakUserClient.ExecuteActionsEmailAsync(
            Realm,
            userId.Value.ToString(),
            request,
            ct);

        return Result.Success();
    }

    public async Task DeleteKeycloakUserAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            var response = await keycloakUserClient.DeleteUserWithResponseAsync(Realm, userId.ToString(), ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "ORPHANED KEYCLOAK USER {UserId} — manual cleanup required", userId);
        }
    }

    private async Task<Guid?> FindKeycloakUserIdByEmailAsync(string email, CancellationToken ct)
    {
        GetUsersRequestParameters parameters = new() { Email = email, Exact = true };
        try
        {
            var users = await keycloakUserClient.GetUsersAsync(Realm, parameters, ct);
            var match = users.FirstOrDefault();
            return match?.Id is null ? null : Guid.Parse(match.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to look up Keycloak user by email {Email}", email);
            return null;
        }
    }
}