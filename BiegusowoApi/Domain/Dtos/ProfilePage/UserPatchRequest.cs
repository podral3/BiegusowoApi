namespace BiegusowoApi.Domain.Dtos.ProfilePage;

public class UserPatchRequest
{
    public string DisplayName { get; set; } = default!;
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string City { get; set; } = default!;
    public int VoivodeshipId { get; set; }
}
