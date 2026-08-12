namespace BiegusowoApi.Shared.Options;

public class SupabseJwtOptions
{
    public string Url { get; set; } = string.Empty;
    public bool ValidateIssuerSigningKey { get; set; }
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public TimeSpan ClockSkew { get; set; }
}


