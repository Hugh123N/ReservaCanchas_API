using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Dashboard;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Dashboard;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Dashboard;

public class GetDashboardAdminQueryHandler(
    IRepository<Entity.Proveedor> proveedores,
    IRepository<Entity.ProveedorPlan> planes,
    IRepository<Entity.PagoPlan> pagos,
    IRepository<Entity.EstadoPago> estadosPago,
    IRepository<Entity.Cancha> canchas,
    IUserIdentity identity) : QueryHandlerBase<GetDashboardAdminQuery, GetDashboardAdminDto>
{
    protected override async Task<ResponseDto<GetDashboardAdminDto>> HandleQuery(GetDashboardAdminQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetDashboardAdminDto>();
        if (!identity.GetCurrentUserClaims().Any(c => (c.Type == "Roles" || c.Type == System.Security.Claims.ClaimTypes.Role) && c.Value == Constants.Role.Admin))
        {
            response.AddErrorResult("Este dashboard es exclusivo del administrador.");
            return response;
        }
        if (request.Meses is not (6 or 12))
        {
            response.AddErrorResult("Selecciona un período de 6 o 12 meses.");
            return response;
        }

        var periodo = new DashboardAdminPeriodo(DateTimeOffset.UtcNow, request.Meses);
        var proveedorQuery = proveedores.FindAllAsNoTracking();
        var planQuery = DashboardAdminConsultas.PlanesActuales(planes.FindAllAsNoTracking(), proveedorQuery, periodo.Ahora);
        var pagoQuery = DashboardAdminConsultas.PagosConfirmados(pagos.FindAllAsNoTracking(), estadosPago.FindAllAsNoTracking(), periodo.Ahora);
        var canchaQuery = DashboardAdminConsultas.CanchasDelNegocio(canchas.FindAllAsNoTracking());

        var indicadores = await proveedorQuery.GroupBy(_ => 1).Select(g => new DashboardAdminIndicadoresDto
        {
            ProveedoresRegistrados = g.Count(), ProveedoresActivos = g.Count(p => p.Activo && p.IdEstadoProveedorNavigation.Codigo == Constants.ESTADO_PROVEEDOR.Aprobado),
            ProveedoresInactivos = g.Count(p => !p.Activo), ProveedoresSuspendidos = g.Count(p => p.Activo && p.IdEstadoProveedorNavigation.Codigo != Constants.ESTADO_PROVEEDOR.Aprobado),
            NuevosProveedoresMes = g.Count(p => p.CreateDate >= periodo.InicioMes && p.CreateDate <= periodo.Ahora)
        }).SingleOrDefaultAsync(cancellationToken) ?? new();
        var canchasResumen = await canchaQuery.GroupBy(_ => 1).Select(g => new
        {
            Aprobadas = g.Count(c => c.IdEstadoCanchaNavigation.Codigo == Constants.ESTADO_CANCHA.Aprobado),
            Pendientes = g.Count(c => c.IdEstadoCanchaNavigation.Codigo == Constants.ESTADO_CANCHA.Pendiente)
        }).SingleOrDefaultAsync(cancellationToken);
        indicadores.CanchasAprobadas = canchasResumen?.Aprobadas ?? 0;
        indicadores.CanchasPendientes = canchasResumen?.Pendientes ?? 0;

        var resumenPlanes = await DashboardAdminConsultas.ResumenPlanes(planQuery, periodo.HastaProximosSieteDias)
            .SingleOrDefaultAsync(cancellationToken) ?? new();
        indicadores.ProveedoresPlanPago = resumenPlanes.Pago;
        indicadores.ProveedoresPrueba = resumenPlanes.Prueba;
        indicadores.PlanesEnGracia = resumenPlanes.EnGracia;
        indicadores.PlanesSuspendidos = resumenPlanes.Suspendidos;
        var comparacion = await pagoQuery.Where(p => p.FechaPago >= periodo.InicioMesAnterior)
            .GroupBy(_ => 1).Select(g => new
            {
                Actual = g.Sum(p => p.FechaPago >= periodo.InicioMes ? p.Monto : 0m),
                Anterior = g.Sum(p => p.FechaPago < periodo.FinComparableAnterior ? p.Monto : 0m)
            }).SingleOrDefaultAsync(cancellationToken);
        indicadores.CobradoMes = comparacion?.Actual ?? 0;
        indicadores.CobradoMesAnteriorComparable = comparacion?.Anterior ?? 0;
        indicadores.VariacionCobrosPorcentaje = DashboardAdminPeriodo.Variacion(indicadores.CobradoMes, indicadores.CobradoMesAnteriorComparable);

        var cobros = await DashboardAdminConsultas.Mensuales(pagoQuery.Select(p => new DashboardAdminFechaValor
            { Fecha = p.FechaPago!.Value, Valor = p.Monto }), periodo).ToListAsync(cancellationToken);
        var registros = await DashboardAdminConsultas.Mensuales(proveedorQuery.Select(p => new DashboardAdminFechaValor
            { Fecha = p.CreateDate, Valor = 1 }), periodo).ToListAsync(cancellationToken);
        var distribucion = await DashboardAdminConsultas.DistribucionPlanes(planQuery).ToListAsync(cancellationToken);
        foreach (var plan in distribucion)
            plan.Porcentaje = resumenPlanes.Pago > 0 ? Math.Round(plan.Cantidad * 100m / resumenPlanes.Pago, 1) : 0;

        var atencion = new List<DashboardAdminAtencionDto>
        {
            new() { Codigo = "CANCHAS", Titulo = "Canchas pendientes de revisión", Cantidad = indicadores.CanchasPendientes,
                Casos = await DashboardAdminConsultas.CanchasPendientes(canchas.FindAllAsNoTracking()).Take(5).ToListAsync(cancellationToken) }
        };
        foreach (var (codigo, titulo, cantidad) in new[]
        {
            ("PRUEBAS", "Pruebas que terminan en 7 días", resumenPlanes.PruebasPorTerminar),
            ("GRACIA", "Planes en período de gracia", resumenPlanes.EnGracia),
            ("SUSPENDIDOS", "Planes suspendidos", resumenPlanes.Suspendidos),
            ("VENCIMIENTOS", "Vencen en 7 días sin renovación automática", resumenPlanes.Vencimientos),
            ("CANCELACIONES", "Cancelaciones programadas", resumenPlanes.Cancelaciones)
        })
        {
            atencion.Add(new DashboardAdminAtencionDto
            {
                Codigo = codigo, Titulo = titulo, Cantidad = cantidad,
                Casos = cantidad == 0 ? new() : await DashboardAdminConsultas.CasosPlanes(planQuery, codigo, periodo.HastaProximosSieteDias)
                    .Take(5).ToListAsync(cancellationToken)
            });
        }
        response.UpdateData(new GetDashboardAdminDto
        {
            GeneradoEn = periodo.Ahora, Meses = request.Meses, Indicadores = indicadores,
            EvolucionMensual = periodo.CompletarMeses(cobros, registros), ProveedoresPorPlan = distribucion, Atencion = atencion
        });
        return response;
    }
}
