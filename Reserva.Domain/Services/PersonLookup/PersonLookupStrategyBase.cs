using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace Reserva.Domain.Services.PersonLookup;

public abstract class PersonLookupStrategyBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsSnapshot<PersonLookupOptions> _options;

    protected PersonLookupStrategyBase(
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<PersonLookupOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    protected PersonLookupProviderOptions GetProviderOptions(string providerName)
    {
        if (!_options.Value.Providers.TryGetValue(providerName, out var options) || !options.Enabled)
            throw new PersonLookupException("El proveedor de consulta está deshabilitado o no está configurado.");

        if (string.IsNullOrWhiteSpace(options.Token) || options.Token.Contains('\r') || options.Token.Contains('\n'))
            throw new PersonLookupException("Falta configurar un token válido para el proveedor de consulta.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri) ||
            baseUri.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(baseUri.Query) || !string.IsNullOrEmpty(baseUri.Fragment) ||
            !string.IsNullOrEmpty(baseUri.UserInfo))
            throw new PersonLookupException("La URL del proveedor de consulta no está configurada correctamente.");

        return options;
    }

    protected static Uri BuildUri(PersonLookupProviderOptions options, string relativePath)
        => new(new Uri(options.BaseUrl.TrimEnd('/') + "/"), relativePath);

    protected async Task<T?> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken) where T : class
    {
        using var client = _httpClientFactory.CreateClient(PersonLookupOptions.HttpClientName);
        using var response = await client.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new PersonLookupException("El proveedor rechazó las credenciales de consulta.");

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new PersonLookupException("Se alcanzó el límite de consultas del proveedor. Intenta más tarde.");

        if (!response.IsSuccessStatusCode)
            throw new PersonLookupException("El proveedor de consulta no está disponible en este momento.");

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            ?? throw InvalidResponse();
    }

    protected static void ValidateDocument(string? returnedNumber, string requestedNumber, string? name)
    {
        if (!string.Equals(returnedNumber, requestedNumber, StringComparison.Ordinal) || string.IsNullOrWhiteSpace(name))
            throw InvalidResponse();
    }

    protected static PersonLookupException InvalidResponse()
        => new("El proveedor devolvió una respuesta de consulta inválida.");
}
