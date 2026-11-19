using System.Linq.Expressions;
using Reserva.Common;

namespace Reserva.Domain.Services.GestionAdmin;

public static class NegocioReglas
{
    public const string Activo = "ACTIVO";
    public const string Suspendido = "SUSPENDIDO";
    public const string Inactivo = "INACTIVO";
    public static bool Habilitado(Entity.Proveedor proveedor) => proveedor.Activo &&
        proveedor.IdEstadoProveedorNavigation?.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado;
    public static string Estado(Entity.Proveedor proveedor) => !proveedor.Activo ? Inactivo : Habilitado(proveedor) ? Activo : Suspendido;
    public static readonly Expression<Func<Entity.Cancha, bool>> Publicable = c => c.Activo &&
        c.IdEstadoCanchaNavigation.Codigo == Constants.ESTADO_CANCHA.Aprobado && c.IdProveedorNavigation.Activo &&
        c.IdProveedorNavigation.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado;
    public static bool EsPublicable(Entity.Cancha cancha) => cancha.Activo &&
        cancha.IdEstadoCanchaNavigation?.Codigo == Constants.ESTADO_CANCHA.Aprobado && Habilitado(cancha.IdProveedorNavigation);
}
