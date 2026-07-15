using System.Text.Json;

namespace BiegusowoApi.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string IdentityId { get; set; }
    public string DisplayName { get; set; } 
    public string AvatarUrl { get; set; }
    public string Role { get; set; }
    public string City { get; set; }

    public int VoivodeshipId { get; set; }
    public Voivodeship Voivodeship { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string BackgroundImageUlr { get; set; }
    public string Bio { get; set; }
    public string PhoneNumber { get; set; }
    public JsonDocument Avatar { get; set; }
    public JsonDocument BackgroundImage { get; set; }

}