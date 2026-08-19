using System.Text.Json;

namespace BiegusowoApi.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarFileName { get; set; }
    public string? BackgroundFileName { get; set; }
    public string? City { get; set; }

    public int? VoivodeshipId { get; set; }
    public Voivodeship? Voivodeship { get; set; }

    public bool IsOnboarded { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; } 
}