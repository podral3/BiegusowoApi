namespace BiegusowoApi.Features.Accounts.Dtos;

public class SupabaseUserCreatedPayload
{
    public string Type { get; set; } = default!;
    public string Table { get; set; } = default!;
    public string Schema { get; set; } = default!;
    public SupabaseUserRecord Record { get; set; } = default!;
}