using Microsoft.Extensions.Options;
using Reserva.Dto.Dbo.PersonLookup;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Reserva.Domain.Services.PersonLookup.Providers;

public sealed class FactilizaStrategy : PersonLookupStrategyBase, IPersonLookupStrategy
{
    public string ProviderName => "Factiliza";

    public FactilizaStrategy(IHttpClientFactory httpClientFactory, IOptionsSnapshot<PersonLookupOptions> options)
        : base(httpClientFactory, options)
    {
    }

    public async Task<DniLookupDto?> GetDniAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest($"dni/info/{Uri.EscapeDataString(documentNumber)}");
        var result = await SendAsync<FactilizaResponse<DniData>>(request, cancellationToken);
        var data = GetData(result);
        if (data == null) return null;

        ValidateDocument(data.Number, documentNumber, data.FirstName);
        if (string.IsNullOrWhiteSpace(data.LastName)) throw InvalidResponse();

        return new DniLookupDto
        {
            FirstName = data.FirstName!.Trim(),
            LastName = data.LastName.Trim(),
            MaternalLastName = data.MaternalLastName?.Trim(),
            DocumentNumber = data.Number!
        };
    }

    public async Task<RucLookupDto?> GetRucAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest($"ruc/info/{Uri.EscapeDataString(documentNumber)}");
        var result = await SendAsync<FactilizaResponse<RucData>>(request, cancellationToken);
        var data = GetData(result);
        if (data == null) return null;

        ValidateDocument(data.Number, documentNumber, data.Name);
        return new RucLookupDto { Name = data.Name!.Trim(), DocumentNumber = data.Number! };
    }

    private HttpRequestMessage CreateRequest(string relativePath)
    {
        var options = GetProviderOptions(ProviderName);
        var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(options, relativePath));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Token);
        return request;
    }

    private static T? GetData<T>(FactilizaResponse<T>? response) where T : class
    {
        if (response == null || response.Status == 404) return null;
        if (!response.Success || response.Status != 200 || response.Data == null) throw InvalidResponse();
        return response.Data;
    }

    private sealed class FactilizaResponse<T> where T : class
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    private sealed class DniData
    {
        [JsonPropertyName("numero")]
        public string? Number { get; set; }
        [JsonPropertyName("nombres")]
        public string? FirstName { get; set; }
        [JsonPropertyName("apellido_paterno")]
        public string? LastName { get; set; }
        [JsonPropertyName("apellido_materno")]
        public string? MaternalLastName { get; set; }
    }

    private sealed class RucData
    {
        [JsonPropertyName("numero")]
        public string? Number { get; set; }
        [JsonPropertyName("nombre_o_razon_social")]
        public string? Name { get; set; }
    }
}
