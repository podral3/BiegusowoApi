using System.Text.Json;

namespace BiegusowoApi.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string IdentityId { get; set; }
    public string DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }

    public Guid? AvatarImageId { get; set; }
    public UserImage AvatarImage { get; set; } = null!;

    public Guid? BackgroundImageId { get; set; }
    public UserImage BackgroundImage { get; set; } = null!;

    public string City { get; set; }

    public int VoivodeshipId { get; set; }
    public Voivodeship Voivodeship { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    
    
    

}