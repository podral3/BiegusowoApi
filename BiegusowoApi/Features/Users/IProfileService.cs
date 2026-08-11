using Ardalis.Result;
using BiegusowoApi.Features.Users.Dtos;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace BiegusowoApi.Features.Users;

public interface IProfileService
{
    Task<ProfilePageResponse?> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<ProfilePageResponse?> GetMyProfileAsync(Guid currentUserId, CancellationToken ct = default);

    Task<Result<ProfilePageResponse>> UpdateMyProfileAsync(
        Guid currentUserId,
        JsonPatchDocument<UserPatchRequest> patch,
        CancellationToken ct = default);
}
