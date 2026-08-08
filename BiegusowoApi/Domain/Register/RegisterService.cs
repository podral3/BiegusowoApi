using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Domain.Dtos.User;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using System.Net;

namespace BiegusowoApi.Domain.Register;

public class RegisterService(
    IKeycloakUserClient keycloakUserClient,
    ApplicationDbContext db,
    ILogger<RegisterService> logger) : IRegisterService
{
    private const string Realm = "biegusowo";

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
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
            response.EnsureSuccessStatusCode();

            // Keycloak's create-user endpoint returns 201 with the new user's URL
            // in the Location header, not a JSON body.
            var location = response.Headers.Location
                ?? throw new RegistrationFailedException("Keycloak returned no Location header.");
            keycloakUserId = Guid.Parse(location.Segments[^1]);
        }
        catch (Exception ex) when (ex is not RegistrationFailedException)
        {
            logger.LogError(ex, "Failed to create Keycloak user for {Email}", request.Email);
            throw new RegistrationFailedException("Could not create account.");
        }

        try
        {
            var response = 
                await keycloakUserClient.SendVerifyEmailWithResponseAsync(Realm, keycloakUserId.ToString(), "biegusowo-test", "biegusowo.gekonlab.org/me");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification email for Keycloak user {UserId}", keycloakUserId);
            throw new RegistrationFailedException("Could not send verification email.");
        }

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
        catch (Exception ex)
        {
            logger.LogError(ex, "DB write failed for Keycloak user {UserId}, rolling back", keycloakUserId);
            await DeleteKeycloakUserAsync(keycloakUserId, ct);
            throw new RegistrationFailedException("Could not complete registration.");
        }

        return new RegisterResult(user.Id);
    }

    public async Task DeleteKeycloakUserAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            var response = 
                await keycloakUserClient.DeleteUserWithResponseAsync(Realm, userId.ToString(), ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // This is the state we want to avoid silently — log loud enough to alert on.
            logger.LogCritical(ex, "ORPHANED KEYCLOAK USER {UserId} — manual cleanup required", userId);
        }
    }
}