using System.Linq.Expressions;
using Reserva.Common;
using Reserva.Dto.Dbo.Dashboard;

namespace Reserva.Domain.Services.Dashboard;

public static class DashboardAdminConsultas
{
    public static IQueryable<Entity.PagoPlan> PagosConfirmados(IQueryable<Entity.PagoPlan> pagos,
        IQueryable<Entity.EstadoPago> estados, DateTimeOffset ahora)
    {
        var confirmados = pagos.Where(p => p.Activo && p.Monto > 0 && p.Moneda == Constants.CURRENCY.PEN &&
            p.FechaPago.HasValue && p.FechaPago <= ahora &&
            estados.Any(e => e.IdEstadoPago == p.IdEstadoPago && e.Codigo == Constants.ESTADO_PAGO.Pagado));
        // Un webhook repetido puede registrar el mismo cargo más de una vez, incluso en otro mes.
        return confirmados.Where(p => p.CulqiChargeId == null || p.CulqiChargeId.Trim() == "" ||
            !confirmados.Any(anterior => anterior.IdPagoPlan < p.IdPagoPlan && anterior.CulqiChargeId == p.CulqiChargeId));
    }

    public static IQueryable<DashboardAdminPlanActual> PlanesActuales(IQueryable<Entity.ProveedorPlan> planes,
        IQueryable<Entity.Proveedor> proveedores, DateTimeOffset ahora)
    {
        var actuales = planes.Where(p => p.Activo && p.EsActual &&
            !planes.Any(otro => otro.Activo && otro.EsActual && otro.IdProveedor == p.IdProveedor && otro.IdProveedorPlan > p.IdProveedorPlan));
        return from plan in actuales
               join proveedor in proveedores.Where(p => p.Activo && p.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado) on plan.IdProveedor equals proveedor.IdProveedor
               select new DashboardAdminPlanActual
               {
                   IdProveedorPlan = plan.IdProveedorPlan, IdPlane = plan.IdPlane,
                   Proveedor = proveedor.RazonSocial != null && proveedor.RazonSocial.Trim() != ""
                       ? proveedor.RazonSocial : (proveedor.Nombres + " " + proveedor.Apellidos).Trim(),
                   Plan = plan.IdPlaneNavigation.Nombre, Estado = plan.Estado,
                   FechaFin = plan.FechaFin, GracePeriodHasta = plan.GracePeriodHasta,
                   CancelAtPeriodEnd = plan.CancelAtPeriodEnd, AutoRenovacion = plan.AutoRenovacion,
                   PruebaVigente = plan.EsPruebaGratis && plan.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE && plan.FechaInicio <= ahora && plan.FechaFin > ahora,
                   PagoVigente = !plan.EsPruebaGratis && plan.IdPlanTarifaNavigation.Precio > 0 &&
                       plan.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE && plan.FechaInicio <= ahora && plan.FechaFin > ahora
               };
    }

    public static IQueryable<DashboardAdminPlanesResumen> ResumenPlanes(IQueryable<DashboardAdminPlanActual> planes, DateTimeOffset hasta) =>
        planes.GroupBy(_ => 1).Select(g => new DashboardAdminPlanesResumen
        {
            Pago = g.Count(p => p.PagoVigente), Prueba = g.Count(p => p.PruebaVigente),
            PruebasPorTerminar = g.Count(p => p.PruebaVigente && p.FechaFin <= hasta),
            EnGracia = g.Count(p => p.Estado == Constants.ESTADO_PROV_PLAN.GRACE || p.Estado == Constants.ESTADO_PROV_PLAN.PAST_DUE),
            Suspendidos = g.Count(p => p.Estado == Constants.ESTADO_PROV_PLAN.SUSPENDED),
            Vencimientos = g.Count(p => p.PagoVigente && !p.AutoRenovacion && !p.CancelAtPeriodEnd && p.FechaFin <= hasta),
            Cancelaciones = g.Count(p => (p.PagoVigente || p.PruebaVigente) && p.CancelAtPeriodEnd)
        });

    public static IQueryable<DashboardAdminPlanDto> DistribucionPlanes(IQueryable<DashboardAdminPlanActual> planes) =>
        planes.Where(p => p.PagoVigente).GroupBy(p => new { p.IdPlane, p.Plan })
            .Select(g => new DashboardAdminPlanDto { Nombre = g.Key.Plan, Cantidad = g.Count() })
            .OrderByDescending(p => p.Cantidad).ThenBy(p => p.Nombre);

    public static IQueryable<DashboardAdminCasoDto> CasosPlanes(IQueryable<DashboardAdminPlanActual> planes, string codigo, DateTimeOffset hasta)
    {
        var seleccion = codigo switch
        {
            "PRUEBAS" => planes.Where(p => p.PruebaVigente && p.FechaFin <= hasta),
            "GRACIA" => planes.Where(p => p.Estado == Constants.ESTADO_PROV_PLAN.GRACE || p.Estado == Constants.ESTADO_PROV_PLAN.PAST_DUE),
            "SUSPENDIDOS" => planes.Where(p => p.Estado == Constants.ESTADO_PROV_PLAN.SUSPENDED),
            "VENCIMIENTOS" => planes.Where(p => p.PagoVigente && !p.AutoRenovacion && !p.CancelAtPeriodEnd && p.FechaFin <= hasta),
            "CANCELACIONES" => planes.Where(p => (p.PagoVigente || p.PruebaVigente) && p.CancelAtPeriodEnd),
            _ => throw new ArgumentOutOfRangeException(nameof(codigo))
        };
        return seleccion.Select(p => new DashboardAdminCasoDto
        {
            Proveedor = p.Proveedor, Plan = p.Plan,
            Fecha = codigo == "GRACIA" ? p.GracePeriodHasta ?? p.FechaFin : p.FechaFin
        }).OrderBy(p => p.Fecha).ThenBy(p => p.Proveedor);
    }

    public static IQueryable<Entity.Cancha> CanchasDelNegocio(IQueryable<Entity.Cancha> canchas) =>
        canchas.Where(c => c.Activo && c.IdProveedorNavigation.Activo && c.IdProveedorNavigation.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado);

    public static IQueryable<DashboardAdminCasoDto> CanchasPendientes(IQueryable<Entity.Cancha> canchas) =>
        CanchasDelNegocio(canchas).Where(c => c.IdEstadoCanchaNavigation.Codigo == Constants.ESTADO_CANCHA.Pendiente)
            .OrderBy(c => c.CreateDate).ThenBy(c => c.IdCancha)
            .Select(c => new DashboardAdminCasoDto
            {
                Proveedor = c.IdProveedorNavigation.RazonSocial != null && c.IdProveedorNavigation.RazonSocial.Trim() != ""
                    ? c.IdProveedorNavigation.RazonSocial : (c.IdProveedorNavigation.Nombres + " " + c.IdProveedorNavigation.Apellidos).Trim(),
                Cancha = c.Nombre, Fecha = c.CreateDate
            });

    public static IQueryable<DashboardAdminMesValor> Mensuales(IQueryable<DashboardAdminFechaValor> datos, DashboardAdminPeriodo periodo)
    {
        // CASE por límites UTC: agrupa en SQL sin cargar operaciones ni depender de funciones de fecha del motor.
        var fila = Expression.Parameter(typeof(DashboardAdminFechaValor), "fila");
        Expression indice = Expression.Constant(periodo.Inicios.Length - 1);
        for (var i = periodo.Inicios.Length - 2; i >= 0; i--)
            indice = Expression.Condition(Expression.LessThan(Expression.Property(fila, nameof(DashboardAdminFechaValor.Fecha)),
                Expression.Constant(periodo.Inicios[i + 1])), Expression.Constant(i), indice);
        var clave = Expression.Lambda<Func<DashboardAdminFechaValor, int>>(indice, fila);
        var inicio = periodo.Inicios[0];
        var ahora = periodo.Ahora;
        return datos.Where(d => d.Fecha >= inicio && d.Fecha <= ahora).GroupBy(clave)
            .Select(g => new DashboardAdminMesValor { Indice = g.Key, Valor = g.Sum(d => d.Valor) });
    }
}

public sealed class DashboardAdminFechaValor
{
    public DateTimeOffset Fecha { get; set; }
    public decimal Valor { get; set; }
}

public sealed class DashboardAdminMesValor
{
    public int Indice { get; set; }
    public decimal Valor { get; set; }
}

public sealed class DashboardAdminPlanActual
{
    public int IdProveedorPlan { get; set; }
    public int IdPlane { get; set; }
    public string Proveedor { get; set; } = "";
    public string Plan { get; set; } = "";
    public string Estado { get; set; } = "";
    public DateTimeOffset FechaFin { get; set; }
    public DateTimeOffset? GracePeriodHasta { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public bool AutoRenovacion { get; set; }
    public bool PruebaVigente { get; set; }
    public bool PagoVigente { get; set; }
}

public sealed class DashboardAdminPlanesResumen
{
    public int Pago { get; set; }
    public int Prueba { get; set; }
    public int PruebasPorTerminar { get; set; }
    public int EnGracia { get; set; }
    public int Suspendidos { get; set; }
    public int Vencimientos { get; set; }
    public int Cancelaciones { get; set; }
}
