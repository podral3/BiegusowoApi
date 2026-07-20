using BiegusowoApi.Domain.Dtos.User;

namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public record ProfilePageResponse(
    UserDto User,
    string? AvatarUrl,
    string? BackgroundImageUrl,
    List<ProfilePageListingSummary> Listings,  
    bool HasMoreListings);