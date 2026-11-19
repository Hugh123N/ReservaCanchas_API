using Reserva.Common;
using Reserva.Dto.GestionAdmin;

namespace Reserva.Domain.Services.GestionAdmin;

public static class GestionAdminConsultas
{
    public static IQueryable<ProveedorAdminDto> Proveedores(IQueryable<Entity.Proveedor> proveedores,
        IQueryable<Entity.ProveedorPlan> planes, IQueryable<Entity.Reserva> reservas, DateTimeOffset ahora)
    {
        var actuales = planes.Where(p => p.Activo && p.EsActual && !planes.Any(o => o.Activo && o.EsActual &&
            o.IdProveedor == p.IdProveedor && o.IdProveedorPlan > p.IdProveedorPlan));
        return from proveedor in proveedores
               join plan in actuales on proveedor.IdProveedor equals plan.IdProveedor into grupo
               from plan in grupo.DefaultIfEmpty()
               select new ProveedorAdminDto
               {
                   IdProveedor = proveedor.IdProveedor,
                   Nombre = proveedor.RazonSocial != null && proveedor.RazonSocial.Trim() != "" ? proveedor.RazonSocial : (proveedor.Nombres + " " + proveedor.Apellidos).Trim(),
                   Titular = (proveedor.Nombres + " " + proveedor.Apellidos).Trim(), Documento = proveedor.NroDoc, Ruc = proveedor.Ruc,
                   Email = proveedor.Email, Telefono = proveedor.Telefono,
                   Estado = !proveedor.Activo ? NegocioReglas.Inactivo : proveedor.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado ? NegocioReglas.Activo : NegocioReglas.Suspendido,
                   Registro = proveedor.CreateDate, Version = proveedor.UpdateDate ?? proveedor.CreateDate,
                   Canchas = proveedor.Cancha.Count(c => c.Activo),
                   ReservasFuturas = reservas.Count(r => r.Activo && r.IdCanchaNavigation.IdProveedor == proveedor.IdProveedor && r.FechaReserva >= ahora &&
                       (r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente || r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado)),
                   Plan = plan == null ? null : plan.IdPlaneNavigation.Nombre, EstadoPlan = plan == null ? null : plan.Estado,
                   FinPlan = plan == null ? null : plan.FechaFin, EsPrueba = plan != null && plan.EsPruebaGratis,
                   AutoRenovacion = plan != null && plan.AutoRenovacion && !plan.CancelAtPeriodEnd
               };
    }

    public static IQueryable<CanchaAdminDto> Canchas(IQueryable<Entity.Cancha> canchas) => canchas.Where(c => c.Activo).Select(c => new CanchaAdminDto
    {
        IdCancha = c.IdCancha, IdProveedor = c.IdProveedor, Nombre = c.Nombre,
        Proveedor = c.IdProveedorNavigation.RazonSocial != null && c.IdProveedorNavigation.RazonSocial.Trim() != ""
            ? c.IdProveedorNavigation.RazonSocial : (c.IdProveedorNavigation.Nombres + " " + c.IdProveedorNavigation.Apellidos).Trim(),
        Direccion = c.Direccion, Imagen = c.ImagenCancha.Where(i => i.Activo).OrderByDescending(i => i.EsPrincipal).Select(i => i.UrlImagen).FirstOrDefault(),
        Estado = c.IdEstadoCanchaNavigation.Codigo, Registro = c.CreateDate, Version = c.UpdateDate ?? c.CreateDate,
        ProveedorHabilitado = c.IdProveedorNavigation.Activo && c.IdProveedorNavigation.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado,
        Publicada = c.IdEstadoCanchaNavigation.Codigo == Constants.ESTADO_CANCHA.Aprobado && c.IdProveedorNavigation.Activo &&
            c.IdProveedorNavigation.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado
    });
}
