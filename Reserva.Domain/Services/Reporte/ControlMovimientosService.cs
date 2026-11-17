using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Services.Reporte;

public class ControlMovimientosService
{
    private const int MaxMovimientos = 50000;
    private readonly ReporteAccessService _access;
    private readonly IRepository<Entity.MovimientoPago> _movimientos;
    private readonly IRepository<Entity.MetodoPago> _metodos;
    private readonly IRepository<Entity.Operador> _operadores;
    private readonly IUserIdentity _identity;

    public ControlMovimientosService(ReporteAccessService access, IRepository<Entity.MovimientoPago> movimientos,
        IRepository<Entity.MetodoPago> metodos, IRepository<Entity.Operador> operadores, IUserIdentity identity)
    {
        _access = access; _movimientos = movimientos; _metodos = metodos; _operadores = operadores; _identity = identity;
    }

    public async Task<ResponseDto<FiltrosControlMovimientosDto>> FiltrosAsync(CancellationToken ct)
    {
        var response = new ResponseDto<FiltrosControlMovimientosDto>();
        if (!_access.TieneRol(TipoReporte.Economico)) { response.AddErrorResult("Este reporte es exclusivo del proveedor."); return response; }
        var canchas = await _access.ObtenerCanchasAsync();
        if (canchas == null) { response.AddErrorResult("No tienes acceso al reporte."); return response; }
        var responsables = new Dictionary<Guid, ResponsableReporteDto>();
        var negocio = _identity.GetCurrentUserIdNegocio();
        foreach (var operador in await _operadores.FindByAsNoTrackingAsync(o => o.IdProveedor == negocio))
            if (Guid.TryParse(operador.IdUsuario, out var id)) responsables[id] = new(id,
                $"{operador.Nombres} {operador.Apellidos}".Trim() + (operador.Activo ? " (Operador)" : " (Operador inactivo)"), operador.Activo);
        if (canchas.Count > 0)
        {
            var ids = canchas.Select(c => c.IdCancha).ToList();
            var historicos = await MovimientosReporteConsultas.Alcance(_movimientos.FindAllAsNoTracking(), ids)
                .Where(m => m.IdUsuarioResponsable.HasValue && m.Tipo != "SALDO_INICIAL")
                .Select(m => new { Id = m.IdUsuarioResponsable!.Value,
                    Nombre = m.IdUsuarioResponsableNavigation == null ? null : (m.IdUsuarioResponsableNavigation.FirstName + " " + m.IdUsuarioResponsableNavigation.LastName).Trim(),
                    Activo = m.IdUsuarioResponsableNavigation != null && m.IdUsuarioResponsableNavigation.Activo })
                .Distinct().Take(10001).ToListAsync(ct);
            if (historicos.Count > 10000) { response.AddErrorResult("El catálogo de responsables es demasiado grande."); return response; }
            foreach (var actor in historicos.Where(a => !responsables.ContainsKey(a.Id)))
                responsables[actor.Id] = new(actor.Id, string.IsNullOrWhiteSpace(actor.Nombre) ? $"Usuario {actor.Id}" : actor.Nombre + (actor.Activo ? "" : " (inactivo)"), actor.Activo);
        }
        if (_identity.GetCurrentUserId() is { } propio) responsables[propio] = new(propio, (_identity.GetUserName() ?? "Dueño") + " (Proveedor)", true);
        response.UpdateData(new FiltrosControlMovimientosDto
        {
            Hoy = DateOnly.FromDateTime(DateTimeHelper.ObtenerAhoraLocal(TimezoneUtils.ObtenerZonaHoraria(canchas.FirstOrDefault()?.ZonaHoraria ?? "America/Lima")).DateTime),
            Canchas = canchas,
            Responsables = responsables.Values.OrderBy(r => r.Nombre).ToList(),
            Metodos = (await _metodos.FindByAsNoTrackingAsync(m => true)).OrderBy(m => m.Nombre).Select(m => new MetodoReporteDto(m.IdMetodoPago, m.Nombre)).ToList()
        });
        return response;
    }

    public async Task<ResponseDto<ControlMovimientosDto>> ObtenerAsync(FiltroControlMovimientosDto filtro, CancellationToken ct)
    {
        var response = new ResponseDto<ControlMovimientosDto>();
        if (!_access.TieneRol(TipoReporte.Economico)) { response.AddErrorResult("Este reporte es exclusivo del proveedor."); return response; }
        var canchas = await _access.ObtenerCanchasAsync();
        if (canchas == null || (filtro.Movimientos.IdCancha.HasValue && !canchas.Any(c => c.IdCancha == filtro.Movimientos.IdCancha)))
        { response.AddErrorResult("Cancha no autorizada para este negocio."); return response; }
        var metodo = filtro.Movimientos.IdMetodoPago.HasValue ? await _metodos.GetByAsNoTrackingAsync(m => m.IdMetodoPago == filtro.Movimientos.IdMetodoPago) : null;
        if (filtro.Movimientos.IdMetodoPago.HasValue && metodo == null) { response.AddErrorResult("Método de pago no encontrado."); return response; }
        var seleccionadas = canchas.Where(c => !filtro.Movimientos.IdCancha.HasValue || c.IdCancha == filtro.Movimientos.IdCancha).ToList();
        var datos = new List<MovimientoReporteData>();
        if (seleccionadas.Count > 0)
        {
            var (desde, hasta) = ReportePeriodo.LimitesUtc(seleccionadas, filtro.Movimientos.FechaDesde, filtro.Movimientos.FechaHasta);
            datos = await MovimientosReporteConsultas.Periodo(_movimientos.FindAllAsNoTracking(), seleccionadas.Select(c => c.IdCancha).ToList(), desde, hasta, filtro.Movimientos)
                .Take(MaxMovimientos + 1).ToListAsync(ct);
            if (datos.Count > MaxMovimientos) { response.AddErrorResult("Se superan 50 000 movimientos. Reduce el período o filtra una cancha/responsable."); return response; }
            Localizar(datos, seleccionadas);
            datos = datos.Where(m => DateOnly.FromDateTime(m.FechaLocal) >= filtro.Movimientos.FechaDesde &&
                DateOnly.FromDateTime(m.FechaLocal) <= filtro.Movimientos.FechaHasta).ToList();
        }
        var resultado = ControlMovimientosBuilder.Construir(datos, seleccionadas, filtro.Movimientos);
        resultado.MetodoFiltro = metodo?.Nombre ?? "Todos los métodos";
        response.UpdateData(resultado);
        return response;
    }

    private static void Localizar(List<MovimientoReporteData> datos, List<CanchaReporteDto> canchas)
    {
        var zonas = canchas.ToDictionary(c => c.IdCancha, c => TimezoneUtils.ObtenerZonaHoraria(c.ZonaHoraria));
        foreach (var m in datos)
        {
            m.FechaLocal = TimeZoneInfo.ConvertTime(m.FechaUtc, zonas[m.IdCancha]).DateTime;
            m.FechaReservaLocal = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(m.FechaReservaUtc, zonas[m.IdCancha]).DateTime);
        }
    }
}
