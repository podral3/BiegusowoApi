using Ardalis.Result;
using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Dtos.ProfilePage;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace BiegusowoApi.Domain.Profile;

public interface IProfileService
{
    Task<ProfilePageResponse?> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<ProfilePageResponse?> GetMyProfileAsync(Guid currentUserId, CancellationToken ct = default);

    Task<Result<ProfilePageResponse>> UpdateMyProfileAsync(
        Guid currentUserId,
        JsonPatchDocument<UserPatchRequest> patch,
        CancellationToken ct = default);
}
