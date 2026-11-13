namespace Reserva.Domain.Services.Reporte;

public record HorarioConsultaReporte(int IdReserva, int IdCancha, int IdHorarioCancha, int DiaSemana, TimeOnly Inicio, TimeOnly? Fin);
public record FranjaConsultaReporte(int IdCancha, int DiaSemana, TimeOnly Inicio, TimeOnly? Fin);

public static class ReporteConsultas
{
    public static IQueryable<ReporteReservaData> Reservas(IQueryable<Entity.Reserva> origen, List<int> ids,
        DateTimeOffset desde, DateTimeOffset hasta, int limite)
        => origen.Where(r => ids.Contains(r.IdCancha) && r.FechaReserva >= desde && r.FechaReserva < hasta)
            .OrderBy(r => r.FechaReserva).ThenBy(r => r.IdReserva).Take(limite)
            .Select(r => new ReporteReservaData
            {
                IdReserva = r.IdReserva, IdCancha = r.IdCancha, Codigo = r.CodigoReserva,
                FechaReserva = r.FechaReserva, Activo = r.Activo, MontoTotal = r.MontoTotal,
                Cliente = r.IdClienteNavigation.Nombres + " " + r.IdClienteNavigation.Apellidos, Telefono = r.IdClienteNavigation.Telefono,
                EstadoCodigo = r.IdEstadoReservaNavigation.Codigo, EstadoNombre = r.IdEstadoReservaNavigation.Nombre,
                Observaciones = r.Observaciones,
                Pago = r.Pago.Where(p => p.Activo).OrderBy(p => p.IdPago).Select(p => new ReportePagoData
                {
                    Total = p.Monto, Cobrado = p.MontoAdelanto, Devuelto = p.MontoReembolso ?? 0,
                    EstadoCodigo = p.IdEstadoPagoNavigation.Codigo, EstadoNombre = p.IdEstadoPagoNavigation.Nombre
                }).FirstOrDefault()
            });

    public static IQueryable<HorarioConsultaReporte> Detalles(IQueryable<Entity.DetalleReserva> origen, List<int> alcance,
        DateTimeOffset inicio, DateTimeOffset fin)
        => origen.Where(d => d.Activo && d.IdHorarioCancha.HasValue &&
                alcance.Contains(d.IdReservaNavigation!.IdCancha) && d.IdReservaNavigation.FechaReserva >= inicio &&
                d.IdReservaNavigation.FechaReserva <= fin)
            .Select(d => new HorarioConsultaReporte(d.IdReserva, d.IdReservaNavigation!.IdCancha, d.IdHorarioCancha!.Value,
                d.IdHorarioCanchaNavigation!.IdDiaSemana, d.IdHorarioCanchaNavigation.IdHoraInicioNavigation.Hora1,
                d.IdHorarioCanchaNavigation.IdHoraFinNavigation == null ? null : d.IdHorarioCanchaNavigation.IdHoraFinNavigation.Hora1));

    public static IQueryable<FranjaConsultaReporte> Configurados(IQueryable<Entity.HorarioCancha> origen, List<int> alcance)
        => origen.Where(h => h.Activo && alcance.Contains(h.IdCancha)).Select(h => new FranjaConsultaReporte(
            h.IdCancha, h.IdDiaSemana, h.IdHoraInicioNavigation.Hora1,
            h.IdHoraFinNavigation == null ? null : h.IdHoraFinNavigation.Hora1));
}
