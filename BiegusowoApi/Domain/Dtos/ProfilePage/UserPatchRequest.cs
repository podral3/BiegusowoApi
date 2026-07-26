namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public record UserPatchRequest(
    string DisplayName,
    string Bio,
    string PhoneNumber,
    string City,
    string VoivodeshipId)
{

}
