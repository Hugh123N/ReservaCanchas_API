using Reserva.Common;
using Reserva.Dto.GestionAdmin;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Services.GestionAdmin;

public class NegocioAccessService(IUserIdentity identity, IRepository<Entity.Proveedor> proveedores,
    IRepository<Entity.Operador> operadores, IRepository<Entity.OperadorCancha> asignaciones)
{
    public bool EsAdmin => identity.GetCurrentUserClaims().Any(c =>
        (c.Type == "Roles" || c.Type == System.Security.Claims.ClaimTypes.Role) && c.Value == Constants.Role.Admin);
    public bool EsProveedor => identity.GetUserRole() == Constants.Role.Proveedor;

    public async Task<Entity.Proveedor?> ProveedorActualAsync()
    {
        if (identity.GetCurrentUserClaims().Any(c => c.Type == "ApplicationCode" && c.Value == Constants.Role.Cliente)) return null;
        var id = identity.GetCurrentUserIdNegocio();
        var usuario = identity.GetCurrentUserId()?.ToString();
        if (id is null or <= 0 || usuario == null) return null;
        if (EsProveedor)
            return await proveedores.GetByAsNoTrackingAsync(p => p.IdProveedor == id && p.IdUsuario == usuario, p => p.IdEstadoProveedorNavigation);
        if (identity.GetUserRole() != Constants.Role.Operador) return null;
        var operador = await operadores.GetByAsNoTrackingAsync(o => o.IdOperador == id && o.IdUsuario == usuario && o.Activo);
        return operador == null ? null : await proveedores.GetByAsNoTrackingAsync(p => p.IdProveedor == operador.IdProveedor, p => p.IdEstadoProveedorNavigation);
    }

    public async Task<bool> PuedeConsultarCanchaAsync(Entity.Cancha cancha)
    {
        if (EsAdmin) return true;
        var proveedor = await ProveedorActualAsync();
        if (proveedor?.IdProveedor != cancha.IdProveedor) return false;
        if (EsProveedor) return true;
        return await asignaciones.GetByAsNoTrackingAsync(a => a.IdOperador == identity.GetCurrentUserIdNegocio() &&
            a.IdCancha == cancha.IdCancha && a.Activo) != null;
    }

    public async Task<bool> PuedeEditarCanchaAsync(Entity.Cancha cancha)
    {
        var proveedor = await ProveedorActualAsync();
        return EsProveedor && proveedor?.IdProveedor == cancha.IdProveedor && NegocioReglas.Habilitado(proveedor);
    }

    public async Task<EstadoNegocioDto> EstadoActualAsync()
    {
        var proveedor = await ProveedorActualAsync();
        var habilitado = proveedor != null && NegocioReglas.Habilitado(proveedor);
        return new EstadoNegocioDto
        {
            Estado = proveedor == null ? NegocioReglas.Inactivo : NegocioReglas.Estado(proveedor), PuedeOperar = habilitado,
            Mensaje = habilitado ? "" : "Tu negocio está suspendido o inactivo. Puedes consultar y atender las reservas existentes, registrar sus cobros y resolver cancelaciones o devoluciones. No puedes crear nuevas reservas ni modificar canchas o contratar planes. Contacta con administración."
        };
    }
}
