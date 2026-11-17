using Reserva.Dto.Reporte;

namespace Reserva.Domain.Services.Reporte;

public static class MovimientosReporteConsultas
{
    public static IQueryable<Entity.MovimientoPago> Alcance(IQueryable<Entity.MovimientoPago> origen, List<int> canchas)
        => origen.Where(m => m.Activo && m.IdPagoNavigation.IdReserva.HasValue &&
            canchas.Contains(m.IdPagoNavigation.IdReservaNavigation!.IdCancha));

    public static IQueryable<MovimientoReporteData> Periodo(IQueryable<Entity.MovimientoPago> origen, List<int> canchas,
        DateTimeOffset desde, DateTimeOffset hasta, FiltroMovimientosReporteDto filtro)
    {
        var query = Alcance(origen, canchas).Where(m => m.CreateDate >= desde && m.CreateDate < hasta);
        if (filtro.IdUsuarioResponsable.HasValue) query = query.Where(m => m.IdUsuarioResponsable == filtro.IdUsuarioResponsable && m.Tipo != "SALDO_INICIAL");
        if (filtro.SoloSinResponsable) query = query.Where(m => !m.IdUsuarioResponsable.HasValue || m.Tipo == "SALDO_INICIAL");
        if (!string.IsNullOrWhiteSpace(filtro.CodigoReserva))
        {
            var codigo = filtro.CodigoReserva.Trim();
            query = query.Where(m => m.IdPagoNavigation.IdReservaNavigation!.CodigoReserva.Contains(codigo));
        }
        if (!string.IsNullOrEmpty(filtro.Tipo)) query = query.Where(m => m.Tipo == filtro.Tipo || m.Tipo == "SALDO_INICIAL");
        // Las correcciones son ajustes globales sin método atribuible al cobro original.
        if (filtro.IdMetodoPago.HasValue) query = query.Where(m => m.IdMetodoPago == filtro.IdMetodoPago || m.Tipo == "CORRECCION" || m.Tipo == "SALDO_INICIAL");
        return Proyectar(query.OrderBy(m => m.CreateDate).ThenBy(m => m.IdMovimientoPago));
    }

    public static IQueryable<MovimientoReporteData> Proyectar(IQueryable<Entity.MovimientoPago> query)
        => query.Select(m => new MovimientoReporteData
        {
            IdMovimiento = m.IdMovimientoPago, IdReserva = m.IdPagoNavigation.IdReserva!.Value,
            IdCancha = m.IdPagoNavigation.IdReservaNavigation!.IdCancha, CodigoReserva = m.IdPagoNavigation.IdReservaNavigation.CodigoReserva,
            FechaUtc = m.CreateDate, FechaReservaUtc = m.IdPagoNavigation.IdReservaNavigation.FechaReserva,
            IdUsuarioResponsable = m.IdUsuarioResponsable, RolResponsable = m.RolResponsable,
            NombreResponsable = m.IdUsuarioResponsableNavigation == null ? null :
                (m.IdUsuarioResponsableNavigation.FirstName + " " + m.IdUsuarioResponsableNavigation.LastName).Trim(),
            NombreAuditoria = m.UserNameCreate, UsuarioActivo = m.IdUsuarioResponsableNavigation != null && m.IdUsuarioResponsableNavigation.Activo,
            Tipo = m.Tipo, Importe = m.Importe, Anterior = m.MontoAnterior, Resultante = m.MontoResultante,
            IdMetodoPago = m.IdMetodoPago, Metodo = m.IdMetodoPagoNavigation == null ? null : m.IdMetodoPagoNavigation.Nombre,
            Referencia = m.ReferenciaOperacion, Motivo = m.Motivo
        });
}
