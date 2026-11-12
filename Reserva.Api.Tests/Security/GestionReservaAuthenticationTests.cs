using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Reserva.Api.Controllers.Dbo;
using Reserva.Api.Security;
using Reserva.Common;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Api.Tests.Security;

public class GestionReservaAuthenticationTests
{
    private const string Issuer = "reserva-auth-tests";
    private const string SigningKey = "test-only-signing-key-not-used-outside-the-test-suite-0123456789abcdef";

    public static IEnumerable<object[]> AccionesGestion()
    {
        foreach (var action in new[] { nameof(ReservaController.Update), nameof(ReservaController.Delete),
            nameof(ReservaController.ConfirmarReservaOperador), nameof(ReservaController.LiberarReservaOperador) })
            yield return new object[] { typeof(ReservaController), action, false };
        foreach (var action in new[] { nameof(PagoController.Historial), nameof(PagoController.RegistrarCobro),
            nameof(PagoController.CorregirRegistro) })
            yield return new object[] { typeof(PagoController), action, action == nameof(PagoController.CorregirRegistro) };
    }

    [Theory]
    [MemberData(nameof(AccionesGestion))]
    public async Task ProveedorConBearer_CargaLaIdentidadYAutorizaLaAccion(Type controller, string action, bool soloProveedor)
    {
        var (result, idUsuario, idNegocio, context) = await AutorizarAsync(controller, action, Constants.Role.Proveedor);
        Assert.True(result.Succeeded);
        Assert.NotNull(idUsuario);
        Assert.Equal(7, idNegocio);
        Assert.True(context.User.IsInRole(Constants.Role.Proveedor));
    }

    [Theory]
    [MemberData(nameof(AccionesGestion))]
    public async Task OperadorConBearer_RespetaLaRestriccionDeCorrecciones(Type controller, string action, bool soloProveedor)
    {
        var (result, idUsuario, _, _) = await AutorizarAsync(controller, action, Constants.Role.Operador);
        Assert.NotNull(idUsuario);
        Assert.Equal(!soloProveedor, result.Succeeded);
        Assert.Equal(soloProveedor, result.Forbidden);
    }

    [Theory]
    [MemberData(nameof(AccionesGestion))]
    public async Task SinBearer_NoLlegaComoUsuarioAnonimoALaGestion(Type controller, string action, bool soloProveedor)
    {
        var (result, idUsuario, _, _) = await AutorizarAsync(controller, action, null);
        Assert.True(result.Challenged);
        Assert.Null(idUsuario);
    }

    [Theory]
    [MemberData(nameof(AccionesGestion))]
    public async Task ClienteAutenticado_NoTienePermisoDeGestion(Type controller, string action, bool soloProveedor)
    {
        var (result, _, _, _) = await AutorizarAsync(controller, action, Constants.Role.Cliente);
        Assert.True(result.Forbidden);
    }

    [Fact]
    public async Task BearerExpirado_NoAutorizaLaEdicion()
    {
        var (result, idUsuario, _, _) = await AutorizarAsync(typeof(ReservaController), nameof(ReservaController.Update),
            Constants.Role.Proveedor, expired: true);
        Assert.True(result.Challenged);
        Assert.Null(idUsuario);
    }

    private static async Task<(PolicyAuthorizationResult Result, Guid? IdUsuario, int? IdNegocio, HttpContext Context)> AutorizarAsync(
        Type controller, string action, string? role, bool expired = false)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConexionString"] = "Server=(local);Database=AuthenticationTests;Trusted_Connection=True;TrustServerCertificate=True",
            ["SecurityOptions:Issuer"] = Issuer,
            ["SecurityOptions:Audience"] = Issuer,
            ["SecurityOptions:SecurityKey"] = SigningKey
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddAuthorization();
        // Usa la configuración real de Identity y JWT; no se resuelve ni se conecta un DbContext.
        services.UseSecurity(configuration, isProduction: true);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = context;
        var idUsuario = Guid.NewGuid();
        if (role != null)
        {
            var token = new JwtSecurityToken(Issuer, Issuer, new[]
            {
                new Claim("UserId", idUsuario.ToString()), new Claim("UserIdNegocio", "7"), new Claim("Roles", role)
            }, notBefore: DateTime.UtcNow.AddMinutes(-10), expires: DateTime.UtcNow.AddMinutes(expired ? -5 : 5),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)), SecurityAlgorithms.HmacSha256));
            context.Request.Headers.Authorization = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
        }
        var attributes = controller.GetMethod(action)!.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .Cast<IAuthorizeData>().ToArray();
        Assert.NotEmpty(attributes);
        var policy = await AuthorizationPolicy.CombineAsync(scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>(), attributes);
        Assert.NotNull(policy);
        var evaluator = scope.ServiceProvider.GetRequiredService<IPolicyEvaluator>();
        var authentication = await evaluator.AuthenticateAsync(policy, context);
        var result = await evaluator.AuthorizeAsync(policy, authentication, context, resource: null);
        var identity = scope.ServiceProvider.GetRequiredService<IUserIdentity>();
        if (authentication.Succeeded) Assert.Equal(idUsuario, identity.GetCurrentUserId());
        return (result, identity.GetCurrentUserId(), identity.GetCurrentUserIdNegocio(), context);
    }
}
