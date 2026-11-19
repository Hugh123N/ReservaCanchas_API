using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Reserva.Domain.Services.Culqi;
using Xunit;

namespace Reserva.Domain.Tests.GestionAdmin;

public class CulqiCancelacionTests
{
    [Theory]
    [InlineData(404, 3, true)]
    [InlineData(400, 4, true)]
    [InlineData(400, 6, true)]
    [InlineData(400, 3, false)]
    [InlineData(500, 4, false)]
    public async Task Reintento_SoloAceptaCancelacionConfirmadaONoExistente(int statusDelete, int estadoSuscripcion, bool resultado)
    {
        using var handler = new PasarelaSimulada((HttpStatusCode)statusDelete, estadoSuscripcion);
        using var http = new HttpClient(handler);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Culqi:SecretKey"] = "test-only", ["Culqi:ApiBaseUrl"] = "https://pasarela.example.invalid"
        }).Build();
        var service = new CulqiService(http, config, NullLogger<CulqiService>.Instance);
        Assert.Equal(resultado, await service.CancelSubscriptionAsync("sub-test"));
    }

    private sealed class PasarelaSimulada(HttpStatusCode estado, int suscripcion) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(new HttpResponseMessage(request.Method == HttpMethod.Delete ? estado : HttpStatusCode.OK)
            { Content = new StringContent($"{{\"status\":{suscripcion}}}") });
    }
}
