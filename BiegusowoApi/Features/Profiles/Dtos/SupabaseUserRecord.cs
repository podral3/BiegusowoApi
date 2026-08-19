using System.Text.Json.Serialization;

namespace BiegusowoApi.Features.Accounts.Dtos;

public class SupabaseUserRecord
{
    public Guid Id { get; set; }
    public string? Email { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
}