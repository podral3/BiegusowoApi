

using BiegusowoApi.Features.Auth;

namespace BiegusowoApi.Features.Users.Dtos;

public record ProfilePageResponse(
    UserDto User,
    string? AvatarUrl,
    string? BackgroundImageUrl,
    List<ProfilePageListingSummary> Listings,  
    bool HasMoreListings);