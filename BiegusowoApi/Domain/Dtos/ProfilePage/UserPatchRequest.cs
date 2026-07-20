namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public record UserPatchRequest(
    string DisplayName,
    decimal Bio,
    string City,
    string VoivodeshipId)
{

}
