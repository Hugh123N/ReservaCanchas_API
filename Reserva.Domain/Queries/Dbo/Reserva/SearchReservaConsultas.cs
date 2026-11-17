using System.Linq.Expressions;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.Reporte;
using Reserva.Repository.Extensions;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Queries.Dbo.Reserva;

public static class SearchReservaConsultas
{
    public static IQueryable<ReservaClienteDto> Filtrar(IQueryable<Entity.Reserva> origen, IQueryable<Entity.Pago> pagos,
        List<CanchaReporteDto> canchas, SearchReservaFilterDto filtro)
    {
        var ids = canchas.Select(c => c.IdCancha).ToList();
        var reservas = origen.Where(r => ids.Contains(r.IdCancha));
        if (filtro.IdCancha.HasValue) reservas = reservas.Where(r => r.IdCancha == filtro.IdCancha);
        var desde = SearchReservaQueryValidator.Desde(filtro);
        var hasta = SearchReservaQueryValidator.Hasta(filtro);
        if (desde.HasValue || hasta.HasValue)
        {
            Expression<Func<Entity.Reserva, bool>> fechas = r => false;
            foreach (var grupo in canchas.GroupBy(c => c.ZonaHoraria))
            {
                var zona = TimezoneUtils.ObtenerZonaHoraria(grupo.Key);
                var idsZona = grupo.Select(c => c.IdCancha).ToList();
                Expression<Func<Entity.Reserva, bool>> intervalo = r => idsZona.Contains(r.IdCancha);
                if (desde.HasValue)
                {
                    var inicio = DateTimeHelper.NormalizarFechaLocal(desde.Value.ToDateTime(TimeOnly.MinValue), zona).ToUniversalTime();
                    intervalo = intervalo.And(r => r.FechaReserva >= inicio);
                }
                if (hasta.HasValue)
                {
                    var fin = DateTimeHelper.NormalizarFechaLocal(hasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), zona).ToUniversalTime();
                    intervalo = intervalo.And(r => r.FechaReserva < fin);
                }
                fechas = fechas.Or(intervalo);
            }
            reservas = reservas.Where(fechas);
        }
        var query = from r in reservas
                    join p in pagos.Where(p => p.Activo) on (int?)r.IdReserva equals p.IdReserva into cabeceras
                    from p in cabeceras.DefaultIfEmpty()
                    let total = p == null ? r.MontoTotal : p.Monto
                    let cobrado = p == null ? 0m : p.MontoAdelanto
                    select new ReservaClienteDto
                    {
                        IdReserva = r.IdReserva, CodigoReserva = r.CodigoReserva, Fecha = r.FechaReserva, FechaCreacion = r.CreateDate,
                        Anulada = !r.Activo, IdCancha = r.IdCancha, NombreCancha = r.IdCanchaNavigation.Nombre + (r.IdCanchaNavigation.Activo ? "" : " (inactiva)"),
                        DireccionCancha = r.IdCanchaNavigation.Direccion ?? "", TelefonoCancha = r.IdCanchaNavigation.TelefonoCancha,
                        NombreCliente = (r.IdClienteNavigation.Nombres + " " + r.IdClienteNavigation.Apellidos).Trim(), TelefonoCliente = r.IdClienteNavigation.Telefono,
                        CodigoEstadoReserva = r.Activo ? r.IdEstadoReservaNavigation.Codigo : Constants.ESTADO_RESERVA.Cancelado,
                        EstadoReserva = r.Activo ? r.IdEstadoReservaNavigation.Nombre : "Cancelada (anulada)",
                        CodigoEstadoPago = p == null ? Constants.ESTADO_PAGO.Pendiente : p.IdEstadoPagoNavigation.Codigo,
                        EstadoPago = p == null ? "Pendiente (sin cobros)" : p.IdEstadoPagoNavigation.Nombre,
                        Monto = total, MontoAdelanto = cobrado, MontoReembolso = p == null ? 0m : p.MontoReembolso ?? 0m,
                        MontoPendiente = r.Activo && (r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente ||
                            r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado) && total > cobrado ? total - cobrado : 0m,
                        FechaExpiracionPreReserva = r.FechaExpiracionPreReserva
                    };
        if (!string.IsNullOrWhiteSpace(filtro.SearchText))
        {
            var texto = filtro.SearchText.Trim();
            query = query.Where(r => r.CodigoReserva.Contains(texto) || r.NombreCliente!.Contains(texto) ||
                (r.TelefonoCliente != null && r.TelefonoCliente.Contains(texto)));
        }
        if (!string.IsNullOrWhiteSpace(filtro.NombreCancha)) query = query.Where(r => r.NombreCancha.Contains(filtro.NombreCancha));
        if (!string.IsNullOrEmpty(filtro.CodigoEstadoPago)) query = query.Where(r => r.CodigoEstadoPago == filtro.CodigoEstadoPago);
        if (filtro.CodigoEstado == "VIGENTES") query = query.Where(r => !r.Anulada && (r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Pendiente || r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Confirmado));
        else if (filtro.CodigoEstado == "ANULADAS") query = query.Where(r => r.Anulada);
        else if (!string.IsNullOrEmpty(filtro.CodigoEstado)) query = query.Where(r => r.CodigoEstadoReserva == filtro.CodigoEstado);
        // IdProveedor se conserva por compatibilidad de entrada; nunca define la autorización.
        return query;
    }

    public static IQueryable<IndicadoresReservasDto> Indicadores(IQueryable<ReservaClienteDto> query)
        => query.GroupBy(r => 1).Select(g => new IndicadoresReservasDto
        {
            Total = g.Count(), Pendientes = g.Count(r => !r.Anulada && r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Pendiente),
            Confirmadas = g.Count(r => !r.Anulada && r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Confirmado),
            Expiradas = g.Count(r => !r.Anulada && r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Expirado),
            Canceladas = g.Count(r => r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Cancelado), Anuladas = g.Count(r => r.Anulada),
            ConDevolucion = g.Count(r => r.MontoReembolso > 0), SaldoVigente = g.Sum(r => r.MontoPendiente),
            NoPresentadas = g.Count(r => !r.Anulada && r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.NoPresentado),
            Completadas = g.Count(r => !r.Anulada && r.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Completado)
        });

    public static IOrderedQueryable<ReservaClienteDto> Ordenar(IQueryable<ReservaClienteDto> query, IEnumerable<SortParamsDto>? sorts)
    {
        IOrderedQueryable<ReservaClienteDto>? orden = null;
        foreach (var sort in sorts ?? Enumerable.Empty<SortParamsDto>())
        {
            Expression<Func<ReservaClienteDto, object>>? campo = sort.Property?.ToLowerInvariant() switch
            {
                "createdate" => r => r.FechaCreacion, "fechareserva" => r => r.Fecha, "codigoreserva" => r => r.CodigoReserva,
                "idreserva" => r => r.IdReserva, "montototal" => r => r.Monto, _ => null
            };
            if (campo == null) continue;
            orden = orden == null ? sort.Direction == "asc" ? query.OrderBy(campo) : query.OrderByDescending(campo) :
                sort.Direction == "asc" ? orden.ThenBy(campo) : orden.ThenByDescending(campo);
        }
        return (orden ?? query.OrderByDescending(r => r.Fecha)).ThenByDescending(r => r.IdReserva);
    }
}
