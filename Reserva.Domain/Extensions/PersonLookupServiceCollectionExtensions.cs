using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reserva.Domain.Services.PersonLookup;
using Reserva.Domain.Services.PersonLookup.Providers;
using System.Net.Http.Headers;

namespace Reserva.Domain.Extensions;

public static class PersonLookupServiceCollectionExtensions
{
    public static IServiceCollection AddPersonLookup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PersonLookupOptions>()
            .Bind(configuration.GetSection(PersonLookupOptions.SectionName))
            .Validate(options => options.TimeoutSeconds is >= 1 and <= 60,
                "PersonLookup:TimeoutSeconds debe estar entre 1 y 60 segundos.");

        services.AddHttpClient(PersonLookupOptions.HttpClientName, (provider, client) =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<PersonLookupOptions>>().CurrentValue;
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            // APIsPERU autentica mediante query string: el logger HTTP predeterminado expondría el token.
            .RemoveAllLoggers();

        services.AddScoped<IPersonLookupStrategy, ApisPeruPortalStrategy>();
        services.AddScoped<IPersonLookupStrategy, FactilizaStrategy>();
        services.AddScoped<IPersonLookupService, PersonLookupService>();
        return services;
    }
}
