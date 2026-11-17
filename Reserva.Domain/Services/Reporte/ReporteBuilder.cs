using Reserva.Common;
using Reserva.Dto.Reporte;
using Reserva.Domain.Helpers;

namespace Reserva.Domain.Services.Reporte;

public static class ReporteBuilder
{
    public static bool EsVigente(ReporteReservaData r) => r.Activo &&
        (r.EstadoCodigo == Constants.ESTADO_RESERVA.Confirmado || r.EstadoCodigo == Constants.ESTADO_RESERVA.Pendiente);

    public static bool CoincideEstado(ReporteReservaData r, FiltroReporteDto filtro) =>
        (string.IsNullOrEmpty(filtro.EstadoReserva) || (filtro.EstadoReserva == "VIGENTES" ? EsVigente(r) :
            filtro.EstadoReserva == "ANULADAS" ? !r.Activo : r.EstadoCodigo == filtro.EstadoReserva)) &&
        (string.IsNullOrEmpty(filtro.EstadoPago) || (r.Pago?.EstadoCodigo ?? Constants.ESTADO_PAGO.Pendiente) == filtro.EstadoPago);

    public static ReporteDto Construir(TipoReporte tipo, FiltroReporteDto filtro, List<ReporteReservaData> reservas,
        List<ReporteHorarioData> horarios, List<CanchaReporteDto> canchas, List<FranjaReporteData> configurados)
    {
        var reporte = new ReporteDto { FechaDesde = filtro.FechaDesde, FechaHasta = filtro.FechaHasta,
            GeneradoEn = DateTimeOffset.UtcNow,
            Cancha = filtro.IdCancha.HasValue ? canchas.First(c => c.IdCancha == filtro.IdCancha).Nombre : "Todas las canchas autorizadas" };
        if (tipo == TipoReporte.Demanda) ConstruirDemanda(reporte, reservas, horarios, canchas, configurados);
        else ConstruirEconomicoAgenda(reporte, reservas, horarios, canchas, tipo == TipoReporte.AgendaOperador);
        var campos = reporte.Columnas.Select(c => c.Campo).ToHashSet();
        reporte.Filas = reporte.Filas.Select(f => f.Where(c => campos.Contains(c.Key)).ToDictionary(c => c.Key, c => c.Value)).ToList();
        return reporte;
    }

    private static void ConstruirEconomicoAgenda(ReporteDto reporte, List<ReporteReservaData> reservas,
        List<ReporteHorarioData> horarios, List<CanchaReporteDto> canchas, bool agenda)
    {
        reporte.Titulo = agenda ? "Agenda y cobros del día" : "Control económico de reservas";
        reporte.Criterio = "Por fecha local de reserva. Los importes reflejan la situación actual, no la caja por fecha de cobro.";
        var nombres = canchas.ToDictionary(c => c.IdCancha, c => c.Nombre);
        var horariosPorReserva = horarios.GroupBy(h => h.IdReserva).ToDictionary(g => g.Key, g =>
            HorarioHelper.FormatearRangosHorarios(g.Select(h => (h.Inicio, h.Fin)).ToList()));
        foreach (var r in reservas.OrderBy(r => r.FechaLocal).ThenBy(r => r.HoraLocal).ThenBy(r => r.IdReserva))
        {
            var total = r.Pago?.Total ?? r.MontoTotal;
            var cobrado = r.Pago?.Cobrado ?? 0;
            var devuelto = r.Pago?.Devuelto ?? 0;
            reporte.Filas.Add(new()
            {
                ["codigo"] = r.Codigo, ["fecha"] = r.FechaLocal, ["cancha"] = nombres[r.IdCancha],
                ["horario"] = horariosPorReserva.GetValueOrDefault(r.IdReserva, r.HoraLocal.ToString("HH:mm")),
                ["cliente"] = r.Cliente, ["telefono"] = r.Telefono ?? "", ["estadoReserva"] = r.EstadoNombre,
                ["estadoPago"] = r.Pago?.EstadoNombre ?? "Pendiente (sin cobros)", ["total"] = total, ["cobrado"] = cobrado,
                ["devuelto"] = devuelto, ["neto"] = cobrado - devuelto,
                ["saldo"] = EsVigente(r) ? Math.Max(0, total - cobrado) : 0, ["observaciones"] = r.Observaciones ?? ""
            });
        }
        reporte.Columnas = agenda ? new()
        {
            new("horario", "Horario"), new("cancha", "Cancha"), new("codigo", "Código"), new("cliente", "Cliente"),
            new("telefono", "Teléfono"), new("estadoReserva", "Reserva"), new("estadoPago", "Pago"),
            new("total", "Total", "moneda"), new("cobrado", "Cobrado", "moneda"), new("saldo", "Por cobrar", "moneda"),
            new("observaciones", "Observaciones")
        } : new()
        {
            new("fecha", "Fecha de reserva", "fecha"), new("codigo", "Código"), new("cancha", "Cancha"),
            new("horario", "Horario"), new("cliente", "Cliente"), new("estadoReserva", "Reserva"), new("estadoPago", "Pago"),
            new("total", "Importe reserva", "moneda"), new("cobrado", "Cobrado acumulado", "moneda"),
            new("devuelto", "Devuelto", "moneda"), new("neto", "Cobrado neto", "moneda"), new("saldo", "Por cobrar", "moneda")
        };
        var vigentes = reservas.Where(EsVigente).ToList();
        decimal Saldo(ReporteReservaData r) => Math.Max(0, (r.Pago?.Total ?? r.MontoTotal) - (r.Pago?.Cobrado ?? 0));
        reporte.Indicadores = agenda ? new()
        {
            new("Reservas vigentes", reservas.Count(EsVigente)),
            new("Pendientes de confirmar", reservas.Count(r => r.Activo && r.EstadoCodigo == Constants.ESTADO_RESERVA.Pendiente)),
            new("Reservas con saldo", vigentes.Count(r => Saldo(r) > 0)),
            new("Por cobrar", vigentes.Sum(Saldo), "moneda")
        } : new()
        {
            new("Reservas", reservas.Count), new("Importe vigente", vigentes.Sum(r => r.Pago?.Total ?? r.MontoTotal), "moneda"),
            new("Cobrado acumulado", reservas.Sum(r => r.Pago?.Cobrado ?? 0), "moneda"),
            new("Devuelto", reservas.Sum(r => r.Pago?.Devuelto ?? 0), "moneda"),
            new("Cobrado neto", reservas.Sum(r => (r.Pago?.Cobrado ?? 0) - (r.Pago?.Devuelto ?? 0)), "moneda"),
            new("Por cobrar", vigentes.Sum(Saldo), "moneda")
        };
    }

    private static void ConstruirDemanda(ReporteDto reporte, List<ReporteReservaData> reservas,
        List<ReporteHorarioData> horarios, List<CanchaReporteDto> canchas, List<FranjaReporteData> configurados)
    {
        reporte.Titulo = "Demanda de canchas y horarios";
        reporte.Criterio = "Horas reservadas confirmadas; pendientes, canceladas y expiradas por separado. No mide asistencia ni porcentaje de ocupación histórica.";
        var nombres = canchas.ToDictionary(c => c.IdCancha, c => c.Nombre);
        var porId = reservas.ToDictionary(r => r.IdReserva);
        var grupos = new Dictionary<(int Cancha, int Dia, TimeOnly Inicio, TimeOnly Fin), List<ReporteHorarioData>>();
        var dias = Enumerable.Range(0, reporte.FechaHasta.DayNumber - reporte.FechaDesde.DayNumber + 1)
            .Select(i => DiaSemana(reporte.FechaDesde.AddDays(i))).ToHashSet();
        foreach (var h in configurados.Where(h => dias.Contains(h.DiaSemana)))
            grupos.TryAdd((h.IdCancha, h.DiaSemana, h.Inicio, h.Fin), new());
        foreach (var h in horarios.Where(h => porId.ContainsKey(h.IdReserva)))
        {
            var key = (h.IdCancha, DiaSemana(porId[h.IdReserva].FechaLocal), h.Inicio, h.Fin);
            if (!grupos.TryGetValue(key, out var items)) grupos[key] = items = new();
            items.Add(h);
        }
        foreach (var (key, items) in grupos)
        {
            var confirmados = items.Where(h => porId[h.IdReserva].EstadoCodigo == Constants.ESTADO_RESERVA.Confirmado && porId[h.IdReserva].Activo).ToList();
            var pendientes = items.Where(h => porId[h.IdReserva].EstadoCodigo == Constants.ESTADO_RESERVA.Pendiente && porId[h.IdReserva].Activo).ToList();
            reporte.Filas.Add(new()
            {
                ["cancha"] = nombres[key.Cancha], ["dia"] = Dias[key.Dia - 1], ["diaOrden"] = key.Dia, ["inicio"] = key.Inicio.ToString("HH:mm"),
                ["fin"] = key.Fin.ToString("HH:mm"), ["confirmadas"] = confirmados.Select(h => h.IdReserva).Distinct().Count(),
                ["horasConfirmadas"] = confirmados.Sum(h => h.Horas),
                ["pendientes"] = pendientes.Select(h => h.IdReserva).Distinct().Count(), ["horasPendientes"] = pendientes.Sum(h => h.Horas),
                ["canceladas"] = items.Where(h => porId[h.IdReserva].EstadoCodigo == Constants.ESTADO_RESERVA.Cancelado || !porId[h.IdReserva].Activo).Select(h => h.IdReserva).Distinct().Count(),
                ["expiradas"] = items.Where(h => porId[h.IdReserva].EstadoCodigo == Constants.ESTADO_RESERVA.Expirado && porId[h.IdReserva].Activo).Select(h => h.IdReserva).Distinct().Count()
            });
        }
        reporte.Filas = reporte.Filas.OrderByDescending(f => (decimal)f["horasConfirmadas"]!).ThenBy(f => f["cancha"])
            .ThenBy(f => f["diaOrden"]).ThenBy(f => f["inicio"]).ToList();
        reporte.Columnas = new()
        {
            new("cancha", "Cancha"), new("dia", "Día"), new("inicio", "Desde"), new("fin", "Hasta"),
            new("confirmadas", "Reservas confirmadas", "numero"), new("horasConfirmadas", "Horas confirmadas", "horas"),
            new("pendientes", "Reservas pendientes", "numero"), new("horasPendientes", "Horas pendientes", "horas"),
            new("canceladas", "Canceladas", "numero"), new("expiradas", "Expiradas", "numero")
        };
        reporte.Indicadores = new()
        {
            new("Reservas confirmadas", reservas.Count(r => r.Activo && r.EstadoCodigo == Constants.ESTADO_RESERVA.Confirmado)),
            new("Horas confirmadas", horarios.Where(h => porId[h.IdReserva].Activo && porId[h.IdReserva].EstadoCodigo == Constants.ESTADO_RESERVA.Confirmado).Sum(h => h.Horas), "horas"),
            new("Pendientes", reservas.Count(r => r.Activo && r.EstadoCodigo == Constants.ESTADO_RESERVA.Pendiente)),
            new("Canceladas", reservas.Count(r => !r.Activo || r.EstadoCodigo == Constants.ESTADO_RESERVA.Cancelado))
        };
    }

    private static int DiaSemana(DateOnly fecha) => fecha.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)fecha.DayOfWeek;
    private static readonly string[] Dias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
}
