using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reserva.Domain.Extensions;
using Reserva.Domain.Services.PersonLookup;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Reserva.Domain.Tests.PersonLookup;

public class PersonLookupTests
{
    private const string Dni = "00000001";
    private const string Ruc = "20000000001";
    private const string TestToken = "test-token+/=";

    [Theory]
    [InlineData("ApisPeruPortal")]
    [InlineData("Factiliza")]
    public async Task Dni_MapsOnlyPersonalFields_AndUsesProviderAuthentication(string providerName)
    {
        var body = providerName == "ApisPeruPortal"
            ? """{"dni":"00000001","nombres":" NOMBRES ","apellidoPaterno":"PATERNO","apellidoMaterno":"MATERNO","direccion":"OMITIR","codVerifica":"9"}"""
            : """{"status":200,"success":true,"data":{"numero":"00000001","nombres":" NOMBRES ","apellido_paterno":"PATERNO","apellido_materno":"MATERNO","direccion":"OMITIR"}}""";
        var handler = new StubHandler(body);
        using var container = CreateContainer(providerName, handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.True(response.IsValid);
        Assert.NotNull(response.Data);
        Assert.Equal("NOMBRES", response.Data.FirstName);
        Assert.Equal("PATERNO", response.Data.LastName);
        Assert.Equal("MATERNO", response.Data.MaternalLastName);
        Assert.Equal(Dni, response.Data.DocumentNumber);

        var json = JsonSerializer.SerializeToElement(response.Data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(new[] { "documentNumber", "firstName", "lastName", "maternalLastName" },
            json.EnumerateObject().Select(property => property.Name).OrderBy(name => name));
        Assert.DoesNotContain("OMITIR", json.GetRawText());
        Assert.Equal(1, handler.RequestCount);

        AssertRequest(handler, providerName, "dni", Dni);
    }

    [Theory]
    [InlineData("ApisPeruPortal")]
    [InlineData("Factiliza")]
    public async Task Ruc_HasItsOwnContract_AndUsesOnlyCompanyNameAndDocument(string providerName)
    {
        var body = providerName == "ApisPeruPortal"
            ? """{"ruc":"20000000001","razonSocial":" EMPRESA DE PRUEBA ","direccion":"OMITIR","estado":"ACTIVO"}"""
            : """{"status":200,"success":true,"data":{"numero":"20000000001","nombre_o_razon_social":" EMPRESA DE PRUEBA ","direccion":"OMITIR","estado":"ACTIVO"}}""";
        var handler = new StubHandler(body);
        using var container = CreateContainer(providerName, handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetRucAsync(Ruc);

        Assert.True(response.IsValid);
        Assert.NotNull(response.Data);
        Assert.Equal("EMPRESA DE PRUEBA", response.Data.Name);
        Assert.Equal(Ruc, response.Data.DocumentNumber);
        var json = JsonSerializer.SerializeToElement(response.Data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(new[] { "documentNumber", "name" },
            json.EnumerateObject().Select(property => property.Name).OrderBy(name => name));
        AssertRequest(handler, providerName, "ruc", Ruc);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("1234567", false)]
    [InlineData("123456789", false)]
    [InlineData("1234567A", false)]
    [InlineData("１２３４５６７８", false)]
    [InlineData("00000001?token=otro", false)]
    [InlineData("", true)]
    [InlineData("2000000000", true)]
    [InlineData("200000000001", true)]
    [InlineData("2000000000A", true)]
    public async Task InvalidDocument_DoesNotCallProvider(string number, bool isRuc)
    {
        var handler = new StubHandler("{}");
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPersonLookupService>();

        var valid = isRuc ? (await service.GetRucAsync(number)).IsValid : (await service.GetDniAsync(number)).IsValid;

        Assert.False(valid);
        Assert.Equal(0, handler.RequestCount);
    }

    [Theory]
    [InlineData("ApisPeruPortal", "Enabled", "false")]
    [InlineData("ApisPeruPortal", "Token", "")]
    [InlineData("ApisPeruPortal", "BaseUrl", "http://example.invalid/")]
    [InlineData("ApisPeruPortal", "BaseUrl", "https://example.invalid/?token=secret")]
    [InlineData("Factiliza", "Enabled", "false")]
    [InlineData("Factiliza", "Token", "")]
    [InlineData("Unknown", "Enabled", "true")]
    public async Task InactiveOrMisconfiguredProvider_DoesNotCallAnyProvider(string providerName, string setting, string value)
    {
        var handler = new StubHandler("{}");
        using var container = CreateContainer(providerName, handler,
            settings => settings[$"PersonLookup:Providers:{providerName}:{setting}"] = value);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        Assert.Equal(0, handler.RequestCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.Redirect)]
    public async Task HttpFailure_ReturnsSafeError_WithoutFallback(HttpStatusCode status)
    {
        var handler = new StubHandler("upstream-sensitive-content", status);
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        Assert.Equal(1, handler.RequestCount);
        Assert.DoesNotContain("upstream-sensitive-content", JsonSerializer.Serialize(response));
        Assert.DoesNotContain(TestToken, JsonSerializer.Serialize(response));
    }

    [Theory]
    [InlineData("not-json")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("{\"success\":false,\"message\":\"sensitive-token\"}")]
    [InlineData("{\"dni\":\"99999999\",\"nombres\":\"NOMBRES\",\"apellidoPaterno\":\"PATERNO\"}")]
    [InlineData("{\"dni\":\"00000001\",\"nombres\":\"\",\"apellidoPaterno\":\"PATERNO\"}")]
    public async Task InvalidUpstreamResponse_DoesNotReturnDataOrRawError(string body)
    {
        var handler = new StubHandler(body);
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        Assert.DoesNotContain("sensitive-token", JsonSerializer.Serialize(response));
    }

    [Theory]
    [InlineData(404)]
    [InlineData(401)]
    [InlineData(500)]
    public async Task Factiliza_RecognizesFailureInsideHttp200(int providerStatus)
    {
        var body = $$"""{"status":{{providerStatus}},"success":false,"message":"sensitive-token"}""";
        var handler = new StubHandler(body);
        using var container = CreateContainer("Factiliza", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetRucAsync(Ruc);

        Assert.False(response.IsValid);
        Assert.Null(response.Data);
        Assert.DoesNotContain("sensitive-token", JsonSerializer.Serialize(response));
    }

    [Fact]
    public async Task Timeout_ReturnsSafeError()
    {
        var handler = new StubHandler((_, _) => throw new TaskCanceledException("sensitive-token"));
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.False(response.IsValid);
        Assert.Contains(response.Messages, message => message.Message.Contains("tiempo de espera"));
        Assert.DoesNotContain("sensitive-token", JsonSerializer.Serialize(response));
    }

    [Fact]
    public async Task NetworkFailure_DoesNotExposeRequestUrlOrToken()
    {
        var handler = new StubHandler((_, _) => throw new HttpRequestException("https://upstream.invalid/?token=sensitive-token"));
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.False(response.IsValid);
        Assert.DoesNotContain("sensitive-token", JsonSerializer.Serialize(response));
        Assert.DoesNotContain("upstream.invalid", JsonSerializer.Serialize(response));
    }

    [Fact]
    public async Task RequestCancellation_IsPropagatedToHttpCall()
    {
        using var cancellation = new CancellationTokenSource();
        var handler = new StubHandler((_, token) =>
        {
            cancellation.Cancel();
            token.ThrowIfCancellationRequested();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni, cancellation.Token));
    }

    [Fact]
    public async Task MissingMaternalSurname_IsAllowed()
    {
        var handler = new StubHandler("""{"dni":"00000001","nombres":"NOMBRES","apellidoPaterno":"PATERNO"}""");
        using var container = CreateContainer("ApisPeruPortal", handler);
        using var scope = container.CreateScope();

        var response = await scope.ServiceProvider.GetRequiredService<IPersonLookupService>().GetDniAsync(Dni);

        Assert.True(response.IsValid);
        Assert.Null(response.Data!.MaternalLastName);
    }

    private static ServiceProvider CreateContainer(
        string providerName,
        StubHandler handler,
        Action<Dictionary<string, string?>>? configure = null)
    {
        var settings = new Dictionary<string, string?>
        {
            ["PersonLookup:ActiveProvider"] = providerName,
            ["PersonLookup:TimeoutSeconds"] = "15",
            ["PersonLookup:Providers:ApisPeruPortal:Enabled"] = "true",
            ["PersonLookup:Providers:ApisPeruPortal:BaseUrl"] = "https://apisperu.invalid/api/v1/",
            ["PersonLookup:Providers:ApisPeruPortal:Token"] = TestToken,
            ["PersonLookup:Providers:Factiliza:Enabled"] = "true",
            ["PersonLookup:Providers:Factiliza:BaseUrl"] = "https://factiliza.invalid/v1/",
            ["PersonLookup:Providers:Factiliza:Token"] = TestToken
        };
        configure?.Invoke(settings);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPersonLookup(configuration);
        services.AddHttpClient(PersonLookupOptions.HttpClientName).ConfigurePrimaryHttpMessageHandler(() => handler);
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    private static void AssertRequest(StubHandler handler, string providerName, string documentType, string number)
    {
        Assert.NotNull(handler.Uri);
        if (providerName == "ApisPeruPortal")
        {
            Assert.Equal("apisperu.invalid", handler.Uri.Host);
            Assert.Equal($"/api/v1/{documentType}/{number}", handler.Uri.AbsolutePath);
            Assert.Equal($"?token={Uri.EscapeDataString(TestToken)}", handler.Uri.Query);
            Assert.Null(handler.Authorization);
        }
        else
        {
            Assert.Equal("factiliza.invalid", handler.Uri.Host);
            Assert.Equal($"/v1/{documentType}/info/{number}", handler.Uri.AbsolutePath);
            Assert.Equal(string.Empty, handler.Uri.Query);
            Assert.Equal($"Bearer {TestToken}", handler.Authorization);
        }
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _send;
        public int RequestCount { get; private set; }
        public Uri? Uri { get; private set; }
        public string? Authorization { get; private set; }

        public StubHandler(string body, HttpStatusCode status = HttpStatusCode.OK)
            : this((_, _) => Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            }))
        {
        }

        public StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) => _send = send;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            Uri = request.RequestUri;
            Authorization = request.Headers.Authorization?.ToString();
            return _send(request, cancellationToken);
        }
    }
}
