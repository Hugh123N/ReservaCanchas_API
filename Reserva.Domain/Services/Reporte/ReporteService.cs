using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Services.Reporte;

public class ReporteService
{
    private const int MaxReservas = 25000;
    private const int MaxHorarios = 250000;
    private readonly ReporteAccessService _access;
    private readonly IRepository<Entity.Reserva> _reservas;
    private readonly IRepository<Entity.DetalleReserva> _detalles;
    private readonly IRepository<Entity.HorarioCancha> _horarios;
    private readonly IRepository<Entity.EstadoReserva> _estadosReserva;
    private readonly IRepository<Entity.EstadoPago> _estadosPago;

    public ReporteService(ReporteAccessService access, IRepository<Entity.Reserva> reservas,
        IRepository<Entity.DetalleReserva> detalles, IRepository<Entity.HorarioCancha> horarios,
        IRepository<Entity.EstadoReserva> estadosReserva, IRepository<Entity.EstadoPago> estadosPago)
    {
        _access = access; _reservas = reservas; _detalles = detalles; _horarios = horarios;
        _estadosReserva = estadosReserva; _estadosPago = estadosPago;
    }

    public async Task<ResponseDto<FiltrosReporteDto>> ObtenerFiltrosAsync()
    {
        var response = new ResponseDto<FiltrosReporteDto>();
        var canchas = await _access.ObtenerCanchasAsync();
        if (canchas == null) { response.AddErrorResult("No tienes acceso a reportes."); return response; }
        var zona = TimezoneUtils.ObtenerZonaHoraria(canchas.FirstOrDefault()?.ZonaHoraria ?? "America/Lima");
        response.UpdateData(new FiltrosReporteDto
        {
            Hoy = DateOnly.FromDateTime(DateTimeHelper.ObtenerAhoraLocal(zona).DateTime), Canchas = canchas,
            EstadosReserva = (await _estadosReserva.FindByAsNoTrackingAsync(e => e.Activo)).OrderBy(e => e.Codigo)
                .Select(e => new OpcionReporteDto(e.Codigo, e.Nombre)).ToList(),
            EstadosPago = (await _estadosPago.FindByAsNoTrackingAsync(e => e.Activo)).OrderBy(e => e.Codigo)
                .Select(e => new OpcionReporteDto(e.Codigo, e.Nombre)).ToList()
        });
        return response;
    }

    public async Task<ResponseDto<ReporteDto>> ObtenerAsync(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<ReporteDto>();
        if (!_access.TieneRol(tipo)) { response.AddErrorResult("No tienes permiso para este reporte."); return response; }
        var canchas = await _access.ObtenerCanchasAsync();
        if (canchas == null || (filtro.IdCancha.HasValue && !canchas.Any(c => c.IdCancha == filtro.IdCancha)))
        {
            response.AddErrorResult("La cancha no existe o no está dentro de tus permisos.");
            return response;
        }
        if (filtro.IdCancha.HasValue) canchas = canchas.Where(c => c.IdCancha == filtro.IdCancha).ToList();
        var reservas = await CargarReservasAsync(canchas, filtro, cancellationToken);
        if (reservas == null)
        {
            response.AddErrorResult("El reporte supera 25 000 reservas. Reduce el período o selecciona una cancha.");
            return response;
        }
        reservas = reservas.Where(r => ReporteBuilder.CoincideEstado(r, filtro)).ToList();
        var ids = reservas.Select(r => r.IdReserva).ToList();
        var horarios = new List<ReporteHorarioData>();
        // Se consulta por el rango y el alcance en SQL, sin un IN con miles de IDs.
        if (ids.Count > 0)
        {
            var alcance = canchas.Select(c => c.IdCancha).ToList();
            var inicio = reservas.Min(r => r.FechaReserva);
            var fin = reservas.Max(r => r.FechaReserva);
            var detalles = await ReporteConsultas.Detalles(_detalles.FindAllAsNoTracking(), alcance, inicio, fin)
                .Take(MaxHorarios + 1).ToListAsync(cancellationToken);
            if (detalles.Count > MaxHorarios)
            {
                response.AddErrorResult("El reporte contiene demasiados horarios. Reduce el período o selecciona una cancha.");
                return response;
            }
            var permitidas = ids.ToHashSet();
            horarios = detalles.Where(d => permitidas.Contains(d.IdReserva)).DistinctBy(d => (d.IdReserva, d.IdHorarioCancha))
                .Select(d => new ReporteHorarioData(d.IdReserva, d.IdCancha, d.IdHorarioCancha, d.Inicio, d.Fin ?? d.Inicio.AddMinutes(30))).ToList();
        }
        var configurados = new List<FranjaReporteData>();
        if (tipo == TipoReporte.Demanda && canchas.Count > 0)
        {
            var idsCanchas = canchas.Select(c => c.IdCancha).ToList();
            var franjas = await ReporteConsultas.Configurados(_horarios.FindAllAsNoTracking(), idsCanchas)
                .Take(MaxReservas + 1).ToListAsync(cancellationToken);
            if (franjas.Count > MaxReservas)
            {
                response.AddErrorResult("Hay demasiadas franjas configuradas. Selecciona una cancha.");
                return response;
            }
            configurados = franjas.Select(h => new FranjaReporteData(h.IdCancha, h.DiaSemana, h.Inicio, h.Fin ?? h.Inicio.AddMinutes(30))).ToList();
        }
        var reporte = ReporteBuilder.Construir(tipo, filtro, reservas, horarios, canchas, configurados);
        if (reporte.Filas.Count > MaxReservas)
        {
            response.AddErrorResult("El reporte supera 25 000 filas. Reduce el período o selecciona una cancha.");
            return response;
        }
        reporte.EstadoReserva = string.IsNullOrEmpty(filtro.EstadoReserva) ? "Todos" : filtro.EstadoReserva == "VIGENTES" ? "Pendientes y confirmadas" :
            reservas.FirstOrDefault()?.EstadoNombre ?? filtro.EstadoReserva;
        reporte.EstadoPago = string.IsNullOrEmpty(filtro.EstadoPago) ? "Todos" : reservas.FirstOrDefault()?.Pago?.EstadoNombre ??
            (filtro.EstadoPago == Constants.ESTADO_PAGO.Pendiente ? "Pendiente (incluye reservas sin cobros)" : filtro.EstadoPago);
        response.UpdateData(reporte);
        return response;
    }

    private async Task<List<ReporteReservaData>?> CargarReservasAsync(List<CanchaReporteDto> canchas, FiltroReporteDto filtro, CancellationToken ct)
    {
        if (canchas.Count == 0) return new();
        var zonas = canchas.ToDictionary(c => c.IdCancha, c => TimezoneUtils.ObtenerZonaHoraria(c.ZonaHoraria));
        var (desde, hasta) = ReportePeriodo.LimitesUtc(canchas, filtro.FechaDesde, filtro.FechaHasta);
        var ids = canchas.Select(c => c.IdCancha).ToList();
        var datos = await ReporteConsultas.Reservas(_reservas.FindAllAsNoTracking(), ids, desde, hasta, MaxReservas + 1).ToListAsync(ct);
        if (datos.Count > MaxReservas) return null;
        foreach (var r in datos)
        {
            var local = TimeZoneInfo.ConvertTime(r.FechaReserva, zonas[r.IdCancha]);
            r.FechaLocal = DateOnly.FromDateTime(local.DateTime);
            r.HoraLocal = TimeOnly.FromDateTime(local.DateTime);
            if (!r.Activo) { r.EstadoCodigo = Constants.ESTADO_RESERVA.Cancelado; r.EstadoNombre = "Cancelada (anulada)"; }
        }
        return datos.Where(r => r.FechaLocal >= filtro.FechaDesde && r.FechaLocal <= filtro.FechaHasta).ToList();
    }

}
