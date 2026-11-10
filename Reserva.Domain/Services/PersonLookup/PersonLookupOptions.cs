namespace Reserva.Domain.Services.PersonLookup;

public class PersonLookupOptions
{
    public const string SectionName = "PersonLookup";
    public const string HttpClientName = "PersonLookup";

    public string ActiveProvider { get; set; } = "ApisPeruPortal";
    public int TimeoutSeconds { get; set; } = 15;
    public Dictionary<string, PersonLookupProviderOptions> Providers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class PersonLookupProviderOptions
{
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
