namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public record UserPatchRequest(
    string DisplayName,
    decimal Bio,
    string PhoneNumber,
    string City,
    string VoivodeshipId)
{

}
