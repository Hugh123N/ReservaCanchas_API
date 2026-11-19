using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.Reserva;
using Reserva.Dto.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Services.GestionAdmin;

public class NegocioRestringidoBehavior<TRequest, TResponse>(IUserIdentity identity, NegocioAccessService access)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull where TResponse : ResponseDto, new()
{
    private static readonly HashSet<string> AtencionExistente = new(StringComparer.Ordinal)
    {
        "DeleteReservaCommand", "LiberarReservaOperadorCommand", "ConfirmarReservaOperadorCommand", "UpdateReservaCommand",
        "RegistrarMovimientoPagoCommand", "ConfirmarPagoCommand", "CompletarPagoCommand", "UpdatePagoCommand"
    };

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var rol = identity.GetUserRole();
        var tipo = request.GetType();
        // La misma cuenta puede comprar como cliente público; ese contexto no administra su negocio.
        if (request is CreateReservaCommand && identity.GetCurrentUserClaims().Any(c =>
            c.Type == "ApplicationCode" && c.Value == Constants.Role.Cliente)) return await next(ct);
        if (request is not ICommand || access.EsAdmin || rol is not (Constants.Role.Proveedor or Constants.Role.Operador) ||
            AtencionExistente.Contains(tipo.Name) || tipo.Namespace?.EndsWith(".Token") == true ||
            tipo.Namespace?.EndsWith(".User") == true || tipo.Namespace?.EndsWith(".Usuario") == true && tipo.Name != "ChangeRoleCommand" ||
            tipo.Namespace?.EndsWith(".Cliente") == true || tipo.Namespace?.EndsWith(".CanchaFavorita") == true ||
            tipo.Name == "UpdateProveedorCommand") return await next(ct);
        var proveedor = await access.ProveedorActualAsync();
        if (proveedor != null && NegocioReglas.Habilitado(proveedor)) return await next(ct);
        var response = new TResponse();
        response.AddErrorResult("El negocio está suspendido o inactivo. Solo puedes consultar y atender las reservas existentes. Contacta con administración.");
        return response;
    }
}
