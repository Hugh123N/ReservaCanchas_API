using Microsoft.Extensions.Options;
using Reserva.Dto.Dbo.PersonLookup;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.PersonLookup.Providers;

public sealed class ApisPeruPortalStrategy : PersonLookupStrategyBase, IPersonLookupStrategy
{
    public string ProviderName => "ApisPeruPortal";

    public ApisPeruPortalStrategy(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PersonLookupOptions> options)
        : base(httpClientFactory, options)
    {
    }

    public async Task<DniLookupDto?> GetDniAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var options = GetProviderOptions(ProviderName);
        using var request = new HttpRequestMessage(HttpMethod.Get,
            BuildUri(options, $"dni/{Uri.EscapeDataString(documentNumber)}?token={Uri.EscapeDataString(options.Token)}"));
        var result = await SendAsync<DniResponse>(request, cancellationToken);
        if (result == null) return null;
        if (result.Success == false) throw InvalidResponse();

        ValidateDocument(result.Number, documentNumber, result.FirstName);
        if (string.IsNullOrWhiteSpace(result.LastName)) throw InvalidResponse();

        return new DniLookupDto
        {
            FirstName = result.FirstName!.Trim(),
            LastName = result.LastName.Trim(),
            MaternalLastName = result.MaternalLastName?.Trim(),
            DocumentNumber = result.Number!
        };
    }

    public async Task<RucLookupDto?> GetRucAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var options = GetProviderOptions(ProviderName);
        using var request = new HttpRequestMessage(HttpMethod.Get,
            BuildUri(options, $"ruc/{Uri.EscapeDataString(documentNumber)}?token={Uri.EscapeDataString(options.Token)}"));
        var result = await SendAsync<RucResponse>(request, cancellationToken);
        if (result == null) return null;
        if (result.Success == false) throw InvalidResponse();

        ValidateDocument(result.Number, documentNumber, result.Name);
        return new RucLookupDto { Name = result.Name!.Trim(), DocumentNumber = result.Number! };
    }

    private sealed class DniResponse
    {
        [JsonPropertyName("success")]
        public bool? Success { get; set; }
        [JsonPropertyName("dni")]
        public string? Number { get; set; }
        [JsonPropertyName("nombres")]
        public string? FirstName { get; set; }
        [JsonPropertyName("apellidoPaterno")]
        public string? LastName { get; set; }
        [JsonPropertyName("apellidoMaterno")]
        public string? MaternalLastName { get; set; }
    }

    private sealed class RucResponse
    {
        [JsonPropertyName("success")]
        public bool? Success { get; set; }
        [JsonPropertyName("ruc")]
        public string? Number { get; set; }
        [JsonPropertyName("razonSocial")]
        public string? Name { get; set; }
    }
}
